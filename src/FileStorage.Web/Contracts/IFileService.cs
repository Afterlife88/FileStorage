using System.Threading.Tasks;
using FileStorage.Web.Models;
using Microsoft.AspNetCore.Http;

namespace FileStorage.Web.Contracts
{
    /// <summary>
    /// Managing file entity
    /// </summary>
    public interface IFileService
    {
        ModelState State { get; }
        Task<ModelState> UploadAsync(IFormFile file, int directoryId, string userEmail);
    }
}
