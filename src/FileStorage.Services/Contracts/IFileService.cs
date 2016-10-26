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
        Task<Tuple<Stream, NodeDto>> GetFile(Guid uniqFileId, string callerEmail, int? versionOfFile);
        Task<IEnumerable<NodeDto>> GetUserFiles(string userEmail);
        ServiceState State { get; }
        Task<ServiceState> UploadAsync(IFormFile file, string directoryName, string userEmail);
    }
}
