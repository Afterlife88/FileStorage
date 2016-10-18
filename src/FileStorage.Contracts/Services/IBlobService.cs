using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FileStorage.Contracts.Services
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
        /// <param name="httpContent">data</param>
        /// <returns></returns>
        Task<Stream> UploadFileAsync(HttpContent httpContent);
        /// <summary>
        /// Deleting file from the blob storage
        /// </summary>
        /// <param name="path">path to the file<</param>
        /// <returns></returns>
        Task DeleteFileAsync(string path);
    }
}
