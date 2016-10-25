using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FileStorage.Domain.Entities;
using FileStorage.Domain.Infrastructure.Contracts;
using FileStorage.Domain.Infrastructure.Repositories;
using FileStorage.Web.Contracts;
using FileStorage.Web.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.StaticFiles;

namespace FileStorage.Web.Services
{
    public class FileService : IFileService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IBlobService _blobService;

        public FileService(IUnitOfWork unitOfWork, IBlobService blobService)
        {
            _unitOfWork = unitOfWork;
            _blobService = blobService;
            State = new ModelState();
        }

        /// <summary>
        /// Model state of the executed actions
        /// </summary>
        public ModelState State { get; }

        public async Task<ModelState> UploadAsync(IFormFile file, int directoryId, string userEmail)
        {
            try
            {
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
                    var newFileVersion = new FileVersion
                    {
                        Node = checkIsNodeAlreadyExistByName,
                        Created = DateTime.Now,
                        MD5Hash = md5Hash,
                        PathToFile = generateNameForAzureBlob,
                        Size = file.Length
                    };
                    _unitOfWork.FileVersionRepository.AddFileVersion(newFileVersion);
                    await _unitOfWork.CommitAsync();
                    await _blobService.UploadFileAsync(file, generateNameForAzureBlob);
                    return State;
                }
                // else just create as first file on the system
                var fileNode = new Node
                {
                    Created = DateTime.Now,
                    IsDirectory = false,
                    Owner = owner,
                    Root = baseDirectory,
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
                    Size = file.Length
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


        private bool ValidateNode(ModelState modelState, Node node, ApplicationUser user)
        {
            if (node == null)
            {
                modelState.ErrorMessage = "Root folder is not found!";

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
    }
}
