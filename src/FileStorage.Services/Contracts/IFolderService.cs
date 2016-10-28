using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FileStorage.Domain.Entities;
using FileStorage.Services.DTO;
using FileStorage.Services.Models;

namespace FileStorage.Services.Contracts
{
    public interface IFolderService
    {
        Task<FolderDto> GetFoldersForUserAsync(string userEmail);
        Task<FolderDto> AddFolderAsync(string userEmail, CreateFolderDto folder);
        Task<FolderDto> GetFolderForUserAsync(string userEmail, Guid folderId);
        ServiceState State { get; }
    }
}
