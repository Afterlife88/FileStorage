using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FileStorage.DAL.Contracts;
using FileStorage.Domain.Entities;
using FileStorage.Services.Contracts;
using FileStorage.Services.DTO;
using FileStorage.Services.Models;
using FileStorage.Services.RequestModels;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;

namespace FileStorage.Services.Implementation
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
        public ServiceState State { get; }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="unitOfWork"></param>
        /// <param name="blobService"></param>
        public FileService(IUnitOfWork unitOfWork, IBlobService blobService)
        {
            _unitOfWork = unitOfWork;
            _blobService = blobService;
            State = new ServiceState();
        }

        #region Methods

        public async Task<IEnumerable<FileDto>> GetUserFilesAsync(string userEmail)
        {
            try
            {
                var owner = await _unitOfWork.UserRepository.GetUserAsync(userEmail);
                var files = await _unitOfWork.NodeRepository.GetAllNodesForUserAsync(owner.Id);

                var filesWithoutFolders = files.Where(r => r.IsDirectory == false);
                return Mapper.Map<IEnumerable<Node>, IEnumerable<FileDto>>(filesWithoutFolders);
            }
            catch (Exception ex)
            {
                State.ErrorMessage = ex.Message;
                State.TypeOfError = TypeOfServiceError.ServiceError;
                return null;
            }
        }

        public async Task<Tuple<Stream, FileDto>> GetFileAsync(Guid uniqFileId, string callerEmail, int? versionOfFile)
        {
            try
            {
                var owner = await _unitOfWork.UserRepository.GetUserAsync(callerEmail);
                var fileNode = await _unitOfWork.NodeRepository.GetNodeByIdAsync(uniqFileId);

                // Validate if user have access to file and can edit it
                if (!ValidateAccessToFile(State, fileNode, owner))
                    return null;

                // Version of file not passed - then return last version
                if (versionOfFile == null)
                    return await GetLastVersionOfFile(fileNode);
                return await GetConcreteVersionOfFile(fileNode, versionOfFile.GetValueOrDefault(-1));
            }
            catch (Exception ex)
            {
                State.ErrorMessage = ex.Message;
                State.TypeOfError = TypeOfServiceError.ServiceError;
                return null;
            }
        }

        public async Task<FileDto> UploadAsync(IFormFile file, Guid? directoryUniqId, string userEmail)
        {
            try
            {
                if (file == null)
                {
                    State.ErrorMessage = "No file attached!";
                    State.TypeOfError = TypeOfServiceError.BadRequest;
                    return null;
                }

                var callerUser = await _unitOfWork.UserRepository.GetUserAsync(userEmail);

                Node directoryWhereFileUploadTo;
                if (directoryUniqId == null)
                    directoryWhereFileUploadTo = await _unitOfWork.NodeRepository.GetRootFolderForUserAsync(callerUser.Id);
                else
                    directoryWhereFileUploadTo = await _unitOfWork.NodeRepository.GetFolderByIdForUserAsync(directoryUniqId.Value, callerUser.Id);

                // Validate current Node (folder that file uploading to) 
                if (!ValidateAccessToFolder(State, directoryWhereFileUploadTo, callerUser))
                {
                    return null;
                }
                // Check if file with concrete hash already exist in service
                string md5Hash = GetMD5HashFromFile(file);
                var checkIsFileWithHashExist =
                    await _unitOfWork.FileVersionRepository.GetFileVersionByMd5HashForUserAsync(md5Hash, callerUser.Id);
                if (checkIsFileWithHashExist != null)
                {
                    State.ErrorMessage = "This version of file already exist!";
                    State.TypeOfError = TypeOfServiceError.BadRequest;
                    return null;
                }

                string generateNameForAzureBlob = GenerateNameForTheAzureBlob(md5Hash, file.FileName, userEmail);
                string contentType;
                new FileExtensionContentTypeProvider().TryGetContentType(file.FileName, out contentType);
                if (contentType == null)
                    contentType = "application/octet-stream";

                var allNodesForUser = await _unitOfWork.NodeRepository.GetAllNodesForUserAsync(callerUser.Id);
                var existedFile = allNodesForUser.FirstOrDefault(r => r.Name == file.FileName && r.IsDirectory == false);
                // If file already exist - add new version
                if (existedFile != null)
                {
                    await AddNewVersionOfFileAsync(file, existedFile, md5Hash,
                        generateNameForAzureBlob);
                    return Mapper.Map<Node, FileDto>(existedFile);
                }

                // else just create as first file on the system
                var fileNode = new Node
                {
                    Created = DateTime.Now,
                    IsDirectory = false,
                    Owner = callerUser,
                    Folder = directoryWhereFileUploadTo,
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
                directoryWhereFileUploadTo.Siblings.Add(fileNode);
                // Upload to azure blob
                await _blobService.UploadFileAsync(file, generateNameForAzureBlob);

                await _unitOfWork.CommitAsync();

         
             
                return Mapper.Map<Node, FileDto>(fileNode);
            }
            catch (Exception ex)
            {
                State.ErrorMessage = ex.Message;
                State.TypeOfError = TypeOfServiceError.ServiceError;
                return null;
            }
        }

        public async Task<FileDto> RenameFileAsync(Guid fileUniqId, string newName, string callerEmail)
        {
            try
            {
                var owner = await _unitOfWork.UserRepository.GetUserAsync(callerEmail);
                var fileNode = await _unitOfWork.NodeRepository.GetNodeByIdAsync(fileUniqId);

                // Validate if user have access to file and can edit it
                if (!ValidateAccessToFile(State, fileNode, owner))
                    return null;


                _unitOfWork.NodeRepository.RenameNode(fileNode, newName);
                await _unitOfWork.CommitAsync();
                return Mapper.Map<Node, FileDto>(fileNode);
            }
            catch (Exception ex)
            {

                State.ErrorMessage = ex.Message;
                State.TypeOfError = TypeOfServiceError.ServiceError;
                return null;
            }
        }

        public async Task<FileDto> ReplaceFileAsync(string callerEmail, Guid fileUniqId, ReplaceRequest model)
        {
            try
            {
                var owner = await _unitOfWork.UserRepository.GetUserAsync(callerEmail);
                var fileNode = await _unitOfWork.NodeRepository.GetNodeByIdAsync(fileUniqId);

                var folderNode = await _unitOfWork.NodeRepository.GetNodeByIdAsync(model.DestanationFolderId);

                if (folderNode == null || !folderNode.IsDirectory)
                {
                    State.TypeOfError = TypeOfServiceError.NotFound;
                    State.ErrorMessage = "Requeset folder not found!";
                    return null;
                }

                // Validate if user have access to file and can edit it
                if (!ValidateAccessToFile(State, fileNode, owner))
                    return null;

                _unitOfWork.NodeRepository.ReplaceNodeFolder(fileNode, folderNode);
                await _unitOfWork.CommitAsync();
                return Mapper.Map<Node, FileDto>(fileNode);
            }
            catch (Exception ex)
            {

                State.ErrorMessage = ex.Message;
                State.TypeOfError = TypeOfServiceError.ServiceError;
                return null;
            }
        }

        public async Task<ServiceState> RemoveFileAsync(Guid fileUniqId, string callerEmail)
        {
            try
            {
                var owner = await _unitOfWork.UserRepository.GetUserAsync(callerEmail);
                var fileNode = await _unitOfWork.NodeRepository.GetNodeByIdAsync(fileUniqId);

                // Validate if user have access to file and can edit it
                if (!ValidateAccessToFile(State, fileNode, owner))
                    return null;


                fileNode.IsDeleted = true;
                var deletedNode = new RemovedNode()
                {
                    Node = fileNode,
                    RemovedOn = DateTime.Now,
                    // Set full remove via one month
                    DateOfRemoval = DateTime.Now.AddMonths(1),
                    ExecutorUser = owner
                };

                _unitOfWork.RemovedNodeRepository.AddRemovedNode(deletedNode);

                await _unitOfWork.CommitAsync();
                return State;
            }
            catch (Exception ex)
            {

                State.ErrorMessage = ex.Message;
                State.TypeOfError = TypeOfServiceError.ServiceError;
                return null;
            }
        }

        public async Task<FileDto> RestoreRemovedFileAsync(Guid fileUniqId, string callerEmail)
        {
            try
            {
                var owner = await _unitOfWork.UserRepository.GetUserAsync(callerEmail);
                var checkIsFileAreNotRemoved = await _unitOfWork.NodeRepository.GetNodeByIdAsync(fileUniqId);
                if (checkIsFileAreNotRemoved != null)
                {
                    State.ErrorMessage = "Requested file are not removed!";
                    State.TypeOfError = TypeOfServiceError.BadRequest;
                    return null;
                }

                var getRemovedNode = await _unitOfWork.NodeRepository.GetNodeThatWasRemoved(fileUniqId);

                // Validate if user have access to file and can edit it
                if (!ValidateAccessToFile(State, getRemovedNode, owner))
                    return null;

                getRemovedNode.IsDeleted = false;

                await _unitOfWork.RemovedNodeRepository.DeleteRemovedNodeRecord(getRemovedNode);
                await _unitOfWork.CommitAsync();

                return Mapper.Map<Node, FileDto>(getRemovedNode);
            }
            catch (Exception ex)
            {

                State.ErrorMessage = ex.Message;
                State.TypeOfError = TypeOfServiceError.ServiceError;
                return null;
            }
        }

        #endregion

        #region Helpers methods 

        private async Task<ServiceState> AddNewVersionOfFileAsync(IFormFile newfile, Node existedFile, string hash, string generatedName)
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
        private async Task<Tuple<Stream, FileDto>> GetConcreteVersionOfFile(Node file, int versionOfFile)
        {
            var getVersionOfFile =
                await _unitOfWork.FileVersionRepository.GetFileVersionOfVersionNumber(file, versionOfFile);

            if (getVersionOfFile == null)
            {
                State.TypeOfError = TypeOfServiceError.NotFound;
                State.ErrorMessage = "Requeset version not found!";
                return Tuple.Create<Stream, FileDto>(null, null);
            }
            var streamOfFileFromBlob = await _blobService.DownloadFileAsync(getVersionOfFile.PathToFile);
            if (streamOfFileFromBlob == null)
            {
                State.TypeOfError = TypeOfServiceError.ConnectionError;
                State.ErrorMessage = "Error with getting file from Azure blob storage!";
                return Tuple.Create<Stream, FileDto>(null, null);
            }

            // Set start position of the stream
            streamOfFileFromBlob.Position = 0;
            return Tuple.Create(streamOfFileFromBlob, Mapper.Map<Node, FileDto>(file));
        }
        private async Task<Tuple<Stream, FileDto>> GetLastVersionOfFile(Node file)
        {

            var getLastVersionOfFile = await _unitOfWork.FileVersionRepository.GetLatestFileVersion(file);
            if (getLastVersionOfFile == null)
            {
                State.TypeOfError = TypeOfServiceError.NotFound;
                State.ErrorMessage = "Latest version of file not found!";
                return Tuple.Create<Stream, FileDto>(null, null);
            }
            var streamOfFileFromBlob = await _blobService.DownloadFileAsync(getLastVersionOfFile.PathToFile);
            if (streamOfFileFromBlob == null)
            {
                State.TypeOfError = TypeOfServiceError.ConnectionError;
                State.ErrorMessage = "Error with getting file from Azure blob storage!";
                return Tuple.Create<Stream, FileDto>(null, null);
            }

            // Set start position of the stream
            streamOfFileFromBlob.Position = 0;
            return Tuple.Create(streamOfFileFromBlob, Mapper.Map<Node, FileDto>(file));
        }

        private bool ValidateAccessToFile(ServiceState state, Node node, ApplicationUser user)
        {
            if (node == null)
            {
                state.ErrorMessage = "File not found!";
                state.TypeOfError = TypeOfServiceError.NotFound;
                return state.IsValid;
            }
            if (node.OwnerId != user.Id)
            {
                State.TypeOfError = TypeOfServiceError.Forbidden;
                state.ErrorMessage = "You not have access to this file";

                return state.IsValid;
            }


            return state.IsValid;
        }

        private string GenerateNameForTheAzureBlob(string md5Hash, string fileName, string userEmail)
        {

            return $"{userEmail}_{md5Hash}_{fileName}";
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
        private bool ValidateAccessToFolder(ServiceState modelState, Node node, ApplicationUser user)
        {
            if (node == null)
            {
                modelState.ErrorMessage = "Folder is not found!";
                modelState.TypeOfError = TypeOfServiceError.NotFound;
                return modelState.IsValid;
            }
            if (node.OwnerId != user.Id)
            {
                State.TypeOfError = TypeOfServiceError.Forbidden;
                modelState.ErrorMessage = "You not have access to this folder";

                return modelState.IsValid;
            }
            return modelState.IsValid;
        }
        #endregion

    }
}
