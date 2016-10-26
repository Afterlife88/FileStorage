using System.IO;
using System.Threading.Tasks;
using FileStorage.Services.Contracts;
using FileStorage.Utils;
using Microsoft.AspNetCore.Http;

namespace FileStorage.Services.Services
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

        public async Task UploadFileAsync(IFormFile file, string generatedFileName)
        {
            var blobContainer = AzureCloudHelpers.GetBlobContainer();
            var blob = blobContainer.GetBlockBlobReference(generatedFileName);
            using (var fs = file.OpenReadStream())
                await blob.UploadFromStreamAsync(fs);


        }

        public async Task DeleteFileAsync(string path)
        {
            var container = AzureCloudHelpers.GetBlobContainer();
            var blob = container.GetBlockBlobReference(path);

            await blob.DeleteAsync();
        }
    }
}
