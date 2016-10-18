using System.IO;
using System.Threading.Tasks;
using FileStorage.Contracts.Services;
using FileStorage.Utils;
using Microsoft.AspNetCore.Http;

namespace FileStorage.Cloud
{
    public class AzureBlobService : IBlobService
    {
     

        public async Task<Stream> DownloadFileAsync(string path)
        {
            var containter = AzureCloudHelpers.GetBlobContainer();
            var blob = containter.GetBlockBlobReference(path);

            var ms = new MemoryStream();
            await blob.DownloadToStreamAsync(ms);

            return ms;
        }

        public async Task<string> UploadFileAsync(IFormFile file)
        {
            var blobContainer = AzureCloudHelpers.GetBlobContainer();
            var blob = blobContainer.GetBlockBlobReference(file.FileName);
            using (var fs = file.OpenReadStream())
                await blob.UploadFromStreamAsync(fs);

            return Path.GetFileName(file.FileName);
        }

        public async Task DeleteFileAsync(string path)
        {
            var container = AzureCloudHelpers.GetBlobContainer();
            var blob = container.GetBlockBlobReference(path);

            await blob.DeleteAsync();
        }
    }
}
