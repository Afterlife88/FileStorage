using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using FileStorage.Domain.Entities;
using FileStorage.Web.DTO;
using FileStorage.Web.Models;
using Microsoft.AspNetCore.Http;

namespace FileStorage.Web.Contracts
{
    /// <summary>
    /// Managing file entity
    /// </summary>
    public interface IFileService
    {

        Task<Tuple<Stream, NodeDto>> GetFile(Guid uniqFileId, string callerEmail, int? versionOfFile);
        Task<IEnumerable<NodeDto>> GetUserFiles(string userEmail);
        ModelState State { get; }
        Task<ModelState> UploadAsync(IFormFile file, string directoryName, string userEmail);
    }
}
