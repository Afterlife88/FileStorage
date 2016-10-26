using System;
using System.Collections.Generic;
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
        Task<IEnumerable<NodeDto>> GetUserFiles(string userEmail);
        ModelState State { get; }
        Task<ModelState> UploadAsync(IFormFile file, int directoryId, string userEmail);
    }
}
