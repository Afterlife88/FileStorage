using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileStorage.Services.DTO;
using FileStorage.Services.Models;
using FileStorage.Services.RequestModels;

namespace FileStorage.Services.Contracts
{
    public interface IFolderService
    {
        Task<FolderDto> GetFoldersForUserAsync(string userEmail);
        Task<FolderDto> AddFolderAsync(string userEmail, CreateFolderRequest folder);
        Task<FolderDto> GetFolderForUserAsync(string userEmail, Guid folderId);
        Task<FolderDto> ReplaceFolderAsync(string callerEmail, Guid folderId, ReplaceRequest model);
        Task<FolderDto> RenameFolderAsync(Guid folderId, string newName, string callerEmail);
        ServiceState State { get; }
        Task<IEnumerable<FolderDto>> GetListFolder(string email);
        Task<ServiceState> RemoveFolderAsync(Guid folderId, string callerEmail);

        Task<FolderDto> RestoreRemovedFolderAsync(Guid folderId, string callerEmail);
    }
}
