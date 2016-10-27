using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FileStorage.Services.DTO;
using FileStorage.Services.Models;
using Microsoft.AspNetCore.Http;

namespace FileStorage.Services.Contracts
{
    /// <summary>
    /// Managing file entity
    /// </summary>
    public interface IFileService
    {
        Task<Tuple<Stream, FileDto>> GetFileAsync(Guid uniqFileId, string callerEmail, int? versionOfFile);
        Task<IEnumerable<FileDto>> GetUserFilesAsync(string userEmail);
        ServiceState State { get; }
        Task<FileDto> UploadAsync(IFormFile file, string directoryName, string userEmail);
        Task<FileDto> RenameFileAsync(Guid fileUniqId, string newName, string callerEmail);
        Task<FileDto> ReplaceFileAsync(string callerEmail, Guid fileUniqId, ReplaceFileDto model);
        Task<ServiceState> RemoveFileAsync(Guid fileUniqId, string callerEmail);
    }
}
