using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FileStorage.DAL.Contracts;
using FileStorage.Domain.Entities;
using FileStorage.Services.Contracts;
using FileStorage.Services.DTO;
using FileStorage.Services.Models;

namespace FileStorage.Services.Implementation
{
    public class FolderService : IFolderService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ServiceState State { get; }

        public FolderService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            State = new ServiceState();
        }

        public async Task<FolderDto> GetFoldersForUserAsync(string userEmail)
        {
            try
            {
                var owner = await _unitOfWork.UserRepository.GetUserAsync(userEmail);

                var rootFolder = await _unitOfWork.NodeRepository.GetRootFolderForUserAsync(owner.Id);

                List<FileDto> filesInFolder = new List<FileDto>();
                List<FolderDto> foldersInFolder = new List<FolderDto>();
                //foreach (var child in rootFolder.Siblings)
                //{
                //    if (!child.IsDirectory)
                //        filesInFolder.Add(Mapper.Map<Node, FileDto>(child));
                //    else
                //        foldersInFolder.Add(Mapper.Map<Node, FolderDto>(child));
                //}

                var res = new FolderDto()
                {
                    Files = filesInFolder,
                    Folders = foldersInFolder,
                    OwnerId = owner.Id,
                    FolderName = rootFolder.Name,
                    UniqueFolderId = rootFolder.Id,
                    ParentFolderId = rootFolder.Id.ToString(),
                    Created = rootFolder.Created,
                    ParentFolderName = rootFolder.Name
                };
                //var res = Mapper.Map<Node, FolderDto>(rootFolder);
                DisplayRecurFolderSibling(rootFolder, res);

                return res;

            }
            catch (Exception ex)
            {
                State.ErrorMessage = ex.Message;
                State.TypeOfError = TypeOfServiceError.ServiceError;
                return null;
            }
        }

        private void DisplayRecurFolderSibling(Node node, FolderDto folder)
        {
            // Right now EF core do not support full nested objects loading. So get next object every time
            var nextNode = _unitOfWork.NodeRepository.GetNodeByIdAsync(node.Id).Result;
            if (nextNode.IsDirectory)
            {
                if (folder.Files == null && folder.Files == null)
                {
                    folder.Files = new List<FileDto>();
                    folder.Folders = new List<FolderDto>();
                }
                var siblings = nextNode.Siblings.ToList();
                foreach (var child in siblings)
                {
                    if (!child.IsDirectory)
                        folder.Files.Add(Mapper.Map<Node, FileDto>(child));
                    else
                    {
                        folder.Folders.Add(Mapper.Map<Node, FolderDto>(child));
                        DisplayRecurFolderSibling(child, folder.Folders.FirstOrDefault());
                    }
                }
            }
        }
        public async Task<FolderDto> AddFolderAsync(string userEmail, CreateFolderDto folder)
        {
            try
            {
                var owner = await _unitOfWork.UserRepository.GetUserAsync(userEmail);


                Node parentDirectoryOfFile;
                if (folder.ParentFolderId == null)
                    parentDirectoryOfFile = await _unitOfWork.NodeRepository.GetRootFolderForUserAsync(owner.Id);
                else
                    parentDirectoryOfFile = await _unitOfWork.NodeRepository.GetFolderByIdForUserAsync(folder.ParentFolderId.Value, owner.Id);

                // Validate current Node (folder that file uploading to) 
                if (!ValidateAccessToFolder(State, parentDirectoryOfFile, owner))
                {
                    return null;
                }

                var existedFolder = await _unitOfWork.NodeRepository.GetFolderByNameForUserAsync(folder.Name, owner.Id);
                if (existedFolder != null)
                {
                    State.ErrorMessage = "Folder with this name already exist!";
                    return null;
                }

                var newFolder = new Node()
                {
                    IsDirectory = true,
                    Created = DateTime.Now,
                    Folder = parentDirectoryOfFile,
                    Owner = owner,
                    Name = folder.Name
                };

                _unitOfWork.NodeRepository.AddNode(newFolder);
                parentDirectoryOfFile.Siblings.Add(newFolder);

                await _unitOfWork.CommitAsync();
                //return Mapper.Map<Node, FolderDto>(folder);
                return null;
            }
            catch (Exception ex)
            {
                State.ErrorMessage = ex.Message;
                State.TypeOfError = TypeOfServiceError.ServiceError;
                return null;
            }
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
    }
}
