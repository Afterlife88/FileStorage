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
        Task<Tuple<Stream, NodeDto>> GetFileAsync(Guid uniqFileId, string callerEmail, int? versionOfFile);
        Task<IEnumerable<NodeDto>> GetUserFilesAsync(string userEmail);
        ServiceState State { get; }
        Task<NodeDto> UploadAsync(IFormFile file, string directoryName, string userEmail);
        Task<NodeDto> RenameFileAsync(Guid fileUniqId, string newName, string callerEmail);
        Task<NodeDto> ReplaceFileAsync(string callerEmail, Guid fileUniqId, ReplaceFileDto model);
    }
}
