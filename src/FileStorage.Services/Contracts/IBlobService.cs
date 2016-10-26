using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FileStorage.Services.Contracts
{
    /// <summary>
    /// Contract for the blob behaviour 
    /// </summary>
    public interface IBlobService
    {
        /// <summary>
        /// Downloading file to the blob storage
        /// </summary>
        /// <param name="path">path to the file</param>
        /// <returns></returns>
        Task<Stream> DownloadFileAsync(string path);

        /// <summary>
        /// Uploading file to the blob storage
        /// </summary>
        /// <param name="file">data</param>
        /// <param name="generatedFileName"></param>
        /// <returns></returns>
        Task UploadFileAsync(IFormFile file, string generatedFileName);
        /// <summary>
        /// Deleting file from the blob storage
        /// </summary>
        /// <param name="path">path to the file<</param>
        /// <returns></returns>
        Task DeleteFileAsync(string path);
    }
}
