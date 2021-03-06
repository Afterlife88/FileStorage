﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using FileStorage.DAL.Contracts;
using FileStorage.Domain.Entities;
using FileStorage.Services.Contracts;
using FileStorage.Services.DTO;
using FileStorage.Services.Models;
using FileStorage.Services.RequestModels;

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
        #region Methods
        public async Task<FolderDto> GetFoldersForUserAsync(string userEmail)
        {
            try
            {
                var owner = await _unitOfWork.UserRepository.GetUserAsync(userEmail);
                var rootFolder = await _unitOfWork.NodeRepository.GetRootFolderForUserAsync(owner.Id);

                var res = new FolderDto()
                {
                    OwnerId = owner.Id,
                    Name = rootFolder.Name,
                    UniqueFolderId = rootFolder.Id,
                    ParentFolderId = rootFolder.Id.ToString(),
                    Created = rootFolder.Created,
                    ParentFolderName = rootFolder.Name
                };
                RecursivelyDisplayFolderSibling(rootFolder, res);
                return res;
            }
            catch (Exception ex)
            {
                State.ErrorMessage = ex.Message;
                State.TypeOfError = TypeOfServiceError.ServiceError;
                return null;
            }
        }
        public async Task<IEnumerable<FolderDto>> GetListFolder(string email)
        {
            try
            {
                var owner = await _unitOfWork.UserRepository.GetUserAsync(email);
                var collectionOfNodes = await _unitOfWork.NodeRepository.GetAllNodesForUserAsync(owner.Id);

                var folders = collectionOfNodes.Where(r => r.IsDirectory);

                return Mapper.Map<IEnumerable<Node>, IEnumerable<FolderDto>>(folders);
            }
            catch (Exception ex)
            {

                State.ErrorMessage = ex.Message;
                State.TypeOfError = TypeOfServiceError.ServiceError;
                return null;
            }

        }

        public async Task<ServiceState> RemoveFolderAsync(Guid folderUniqId, string callerEmail)
        {
            try
            {
                var owner = await _unitOfWork.UserRepository.GetUserAsync(callerEmail);
                var nodeToRemove = await _unitOfWork.NodeRepository.GetNodeByIdAsync(folderUniqId);

                if (nodeToRemove == null || !nodeToRemove.IsDirectory)
                {
                    State.ErrorMessage = "Requested folder not found!";
                    State.TypeOfError = TypeOfServiceError.BadRequest;
                    return State;
                }

                RecursiveDelete(nodeToRemove, owner);
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



        public async Task<FolderDto> GetFolderForUserAsync(string userEmail, Guid folderId)
        {
            try
            {
                var owner = await _unitOfWork.UserRepository.GetUserAsync(userEmail);
                var node = await _unitOfWork.NodeRepository.GetNodeByIdAsync(folderId);


                var parentFolder = await _unitOfWork.NodeRepository.GetNodeByIdAsync(node.FolderId.GetValueOrDefault(node.Id));

                // TODO: Later make check with permission manager
                var res = new FolderDto()
                {
                    OwnerId = owner.Id,
                    Name = node.Name,
                    UniqueFolderId = node.Id,
                    ParentFolderId = parentFolder.Id.ToString(),
                    Created = node.Created,
                    ParentFolderName = parentFolder.Name
                };
                RecursivelyDisplayFolderSibling(node, res);
                return res;
            }
            catch (Exception ex)
            {
                State.ErrorMessage = ex.Message;
                State.TypeOfError = TypeOfServiceError.ServiceError;
                return null;
            }
        }
        public async Task<FolderDto> AddFolderAsync(string userEmail, CreateFolderRequest folder)
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
                    State.TypeOfError = TypeOfServiceError.BadRequest;
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
                return Mapper.Map<Node, FolderDto>(newFolder);
            }
            catch (Exception ex)
            {
                State.ErrorMessage = ex.Message;
                State.TypeOfError = TypeOfServiceError.ServiceError;
                return null;
            }
        }

        public async Task<FolderDto> ReplaceFolderAsync(string callerEmail, Guid folderId, ReplaceRequest model)
        {
            try
            {
                // Make something with permission manager
                var owner = await _unitOfWork.UserRepository.GetUserAsync(callerEmail);
                var currentFolder = await _unitOfWork.NodeRepository.GetNodeByIdAsync(folderId);

                var destFolder = await _unitOfWork.NodeRepository.GetNodeByIdAsync(model.DestanationFolderId);


                if (currentFolder == null || !currentFolder.IsDirectory)
                {
                    State.TypeOfError = TypeOfServiceError.NotFound;
                    State.ErrorMessage = "Source folder not found!";
                    return null;
                }
                if (destFolder == null || !destFolder.IsDirectory)
                {
                    State.TypeOfError = TypeOfServiceError.NotFound;
                    State.ErrorMessage = "Destination folder not found!";
                    return null;
                }

                if (destFolder.Folder == currentFolder)
                {
                    State.TypeOfError = TypeOfServiceError.BadRequest;
                    State.ErrorMessage = "Destination folder cannot be inside requested folder";
                    return null;
                }
                currentFolder.Folder = destFolder;
                await _unitOfWork.CommitAsync();
                return Mapper.Map<Node, FolderDto>(currentFolder);
            }
            catch (Exception ex)
            {
                State.ErrorMessage = ex.Message;
                State.TypeOfError = TypeOfServiceError.ServiceError;
                return null;
            }
        }
        public async Task<FolderDto> RenameFolderAsync(Guid fileUniqId, string newName, string callerEmail)
        {
            try
            {
                var owner = await _unitOfWork.UserRepository.GetUserAsync(callerEmail);
                var folder = await _unitOfWork.NodeRepository.GetFolderByIdAsync(fileUniqId);

                // Validate if user have access to file and can edit it
                if (!ValidateAccessToFolder(State, folder, owner))
                    return null;

                _unitOfWork.NodeRepository.RenameNode(folder, newName);
                await _unitOfWork.CommitAsync();
                return Mapper.Map<Node, FolderDto>(folder);
            }
            catch (Exception ex)
            {
                State.ErrorMessage = ex.Message;
                State.TypeOfError = TypeOfServiceError.ServiceError;
                return null;
            }
        }

        public async Task<FolderDto> RestoreRemovedFolderAsync(Guid folderId, string callerEmail)
        {
            try
            {
                var owner = await _unitOfWork.UserRepository.GetUserAsync(callerEmail);
                var folder = await _unitOfWork.NodeRepository.GetNodeThatWasRemoved(folderId);

                if (folder == null || !folder.IsDirectory)
                {
                    State.ErrorMessage = "Requested folder not found!";
                    State.TypeOfError = TypeOfServiceError.BadRequest;
                    return null;
                }

                RecursiveRestore(folder);

                await _unitOfWork.CommitAsync();
                return Mapper.Map<Node, FolderDto>(folder);
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
        private void RecursivelyDisplayFolderSibling(Node node, FolderDto folder)
        {
            // Right now EF core do not support full nested objects loading. So get next object every time
            var nextNode = _unitOfWork.NodeRepository.GetNodeByIdAsync(node.Id).Result;
            if (nextNode.IsDirectory)
            {
                if (folder.Folders == null)
                {
                    folder.Folders = new List<FolderDto>();
                }
                if (folder.Files == null)
                {
                    folder.Files = new List<FileDto>();
                }
                var siblings = nextNode.Siblings.ToList();
                foreach (var child in siblings)
                {
                    if (!child.IsDeleted)
                    {
                        if (!child.IsDirectory)
                            folder.Files.Add(Mapper.Map<Node, FileDto>(child));
                        else
                        {
                            folder.Folders.Add(Mapper.Map<Node, FolderDto>(child));
                            var siblingsFolders = folder.Folders.ToList();
                            foreach (var a in siblingsFolders)
                            {
                                RecursivelyDisplayFolderSibling(child, a);
                            }
                        }
                    }
                }
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

        private void RecursiveDelete(Node parent, ApplicationUser user)
        {

            if (parent.IsDirectory)
            {
                var siblings = parent.Siblings.ToList();
                foreach (var child in siblings)
                {
                    RecursiveDelete(child, user);
                }
            }

            parent.IsDeleted = true;
            _unitOfWork.RemovedNodeRepository.AddRemovedNode(new RemovedNode()
            {
                Node = parent,
                DateOfRemoval = DateTime.Now.AddMonths(1),
                ExecutorUser = user,
                RemovedOn = DateTime.Now
            });
        }

        private void RecursiveRestore(Node parent)
        {

            if (parent.IsDirectory)
            {
                var siblings = parent.Siblings.ToList();
                foreach (var child in siblings)
                {
                    RecursiveRestore(child);
                }
            }

            parent.IsDeleted = false;
            _unitOfWork.RemovedNodeRepository.DeleteRemovedNodeRecord(parent);
        }
        #endregion
    }
}
