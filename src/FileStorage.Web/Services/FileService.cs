using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FileStorage.Domain.Entities;
using FileStorage.Domain.Infrastructure.Contracts;
using FileStorage.Web.Contracts;
using FileStorage.Web.DTO;
using FileStorage.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;

namespace FileStorage.Web.Services
{
    /// <summary>
    /// Service for manage files
    /// </summary>
    public class FileService : IFileService
    {
        #region Variables

        private readonly IUnitOfWork _unitOfWork;
        private readonly IBlobService _blobService;

        #endregion
        /// <summary>
        /// Model state of the executed actions
        /// </summary>
        public ModelState State { get; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="blobService"></param>
        public FileService(IUnitOfWork unitOfWork, IBlobService blobService)
        {
            _unitOfWork = unitOfWork;
            _blobService = blobService;
            State = new ModelState();
        }
        #region Methods

        public async Task<IEnumerable<NodeDto>> GetUserFiles(string userEmail)
        {
            try
            {
                var owner = await _unitOfWork.UserRepository.GetUserAsync(userEmail);
                var files = await _unitOfWork.NodeRepository.GetAllNodesForUser(owner.Id);

                var filesWithoutFolders = files.Where(r => r.IsDirectory == false);
                return Mapper.Map<IEnumerable<Node>, IEnumerable<NodeDto>>(filesWithoutFolders);
            }
            catch (Exception ex)
            {
                State.ErrorMessage = ex.Message;
                return null;
            }
        }

        public async Task<Tuple<Stream, NodeDto>> GetLastVersionOfFile(string fileName)
        {
            try
            {
                var getFileNode = await _unitOfWork.NodeRepository.GetNodeByName(fileName);
                if (getFileNode == null)
                {
                    State.ErrorMessage = "Requested file is not found!";
                    return Tuple.Create<Stream, NodeDto>(null, null);
                }
                var getLastVersionOfFile = await _unitOfWork.FileVersionRepository.GetLatestFileVersion(getFileNode);
                if (getLastVersionOfFile == null)
                {
                    State.ErrorMessage = "Latest version of file not found!";
                    return Tuple.Create<Stream, NodeDto>(null, null);
                }
                var streamOfFileFromBlob = await _blobService.DownloadFileAsync(getLastVersionOfFile.PathToFile);
                if (streamOfFileFromBlob == null)
                {
                    State.ErrorMessage = "Error with getting file from Azure blob storage!";
                    return Tuple.Create<Stream, NodeDto>(null, null);
                }

                // Set start position of the stream
                streamOfFileFromBlob.Position = 0;
                return Tuple.Create(streamOfFileFromBlob, Mapper.Map<Node, NodeDto>(getFileNode));
            }
            catch (Exception ex)
            {
                State.ErrorMessage = ex.Message;
                return null;
            }
        }

        public Task<Tuple<Stream, NodeDto>> GetConcreteVersionofFile(string fileName, int versionOfFile)
        {
            try
            {
                return null;
            }
            catch (Exception ex)
            {
                State.ErrorMessage = ex.Message;
                return null;
            }
        }


        public async Task<ModelState> UploadAsync(IFormFile file, int directoryId, string userEmail)
        {
            try
            {
                if (file == null)
                {
                    State.ErrorMessage = "No file attached!";
                    return State;
                }

                var baseDirectory = await _unitOfWork.NodeRepository.GetNodeById(directoryId);
                var owner = await _unitOfWork.UserRepository.GetUserAsync(userEmail);

                // Validate current Node (folder that file uploading to) 
                if (!ValidateNode(State, baseDirectory, owner))
                {
                    return State;
                }
                // Check if file with concrete hash already exist in service
                string md5Hash = GetMD5HashFromFile(file);
                var checkIsFileWithHashExist = await _unitOfWork.FileVersionRepository.GetFileVersionByMd5HashAsync(md5Hash);
                if (checkIsFileWithHashExist != null)
                {
                    State.ErrorMessage = "This version of file already exist!";
                    return State;
                }

                string generateNameForAzureBlob = GenerateNameForTheAzureBlob(md5Hash, file.FileName);
                string contentType;
                new FileExtensionContentTypeProvider().TryGetContentType(file.FileName, out contentType);
                if (contentType == null)
                    contentType = "none";

                var checkIsNodeAlreadyExistByName = await _unitOfWork.NodeRepository.GetNodeByName(file.FileName);
                // If file already exist - add new version
                if (checkIsNodeAlreadyExistByName != null)
                {
                    await AddNewVersionOfFileAsync(file, checkIsNodeAlreadyExistByName, md5Hash,
                                  generateNameForAzureBlob);
                    return State;
                }

                // else just create as first file on the system
                var fileNode = new Node
                {
                    Created = DateTime.Now,
                    IsDirectory = false,
                    Owner = owner,
                    Folder = baseDirectory,
                    Name = file.FileName,
                    FileVersions = new List<FileVersion>(),
                    ContentType = contentType,
                };
                var fileVersion = new FileVersion
                {
                    Node = fileNode,
                    Created = DateTime.Now,
                    MD5Hash = md5Hash,
                    PathToFile = generateNameForAzureBlob,
                    Size = file.Length,
                    VersionOfFile = 1
                };

                // Add to db
                _unitOfWork.NodeRepository.AddNode(fileNode);
                _unitOfWork.FileVersionRepository.AddFileVersion(fileVersion);
                baseDirectory.Siblings.Add(fileNode);

                await _unitOfWork.CommitAsync();

                // Upload to azure blob
                await _blobService.UploadFileAsync(file, generateNameForAzureBlob);
                return State;
            }
            catch (Exception ex)
            {
                await _unitOfWork.CommitAsync();
                State.ErrorMessage = ex.Message;
                return State;
            }
        }


        #endregion

        #region Helpers methods 

        private async Task<ModelState> AddNewVersionOfFileAsync(IFormFile newfile, Node existedFile, string hash, string generatedName)
        {
            try
            {
                int getLastVersionOfFile =
                     await _unitOfWork.FileVersionRepository.GetNumberOfLastVersionFile(existedFile);
                // Increment version of file
                getLastVersionOfFile++;
                var newFileVersion = new FileVersion
                {
                    Node = existedFile,
                    Created = DateTime.Now,
                    MD5Hash = hash,
                    PathToFile = generatedName,
                    Size = newfile.Length,
                    VersionOfFile = getLastVersionOfFile
                };
                _unitOfWork.FileVersionRepository.AddFileVersion(newFileVersion);
                await _unitOfWork.CommitAsync();
                await _blobService.UploadFileAsync(newfile, generatedName);
                return State;
            }
            catch (Exception ex)
            {
                await _unitOfWork.CommitAsync();
                State.ErrorMessage = ex.Message;
                return State;
            }
        }

        private bool ValidateNode(ModelState modelState, Node node, ApplicationUser user)
        {
            if (node == null)
            {
                modelState.ErrorMessage = "Folder is not found!";

                return modelState.IsValid;
            }
            if (node.OwnerId != user.Id)
            {
                modelState.ErrorMessage = "Access denied";

                return modelState.IsValid;
            }
            return modelState.IsValid;
        }


        private string GenerateNameForTheAzureBlob(string md5Hash, string fileName)
        {
            return $"{md5Hash}_{fileName}";
        }
        private string GetMD5HashFromFile(IFormFile file)
        {
            string md5Hash;
            using (var md5 = MD5.Create())
            {
                using (var stream = file.OpenReadStream())
                {
                    var buffer = md5.ComputeHash(stream);
                    var sb = new StringBuilder();
                    foreach (byte t in buffer)
                    {
                        sb.Append(t.ToString("x2"));
                    }
                    md5Hash = sb.ToString();
                }
            }
            return md5Hash;
        }
        #endregion

    }
}
