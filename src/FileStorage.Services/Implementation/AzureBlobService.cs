using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FileStorage.DAL.Contracts;
using FileStorage.Services.Contracts;
using FileStorage.Services.Models;
using FileStorage.Utils;
using Microsoft.AspNetCore.Http;

namespace FileStorage.Services.Implementation
{
    public class AzureBlobService : IBlobService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AzureBlobService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<Stream> DownloadFileAsync(string path)
        {
            try
            {
                var containter = AzureCloudHelpers.GetBlobContainer();
                var blob = containter.GetBlockBlobReference(path);

                var ms = new MemoryStream();
                await blob.DownloadToStreamAsync(ms);

                return ms;
            }
            catch (Exception)
            {
                throw new AzureException(
                    "Failed to connect to Azure Blob from docker container! Please reboot docker and try again!");
            }
          
        }

        public async Task UploadFileAsync(IFormFile file, string generatedFileName)
        {
            try
            {
                var blobContainer = AzureCloudHelpers.GetBlobContainer();
                var blob = blobContainer.GetBlockBlobReference(generatedFileName);
                using (var fs = file.OpenReadStream())
                    await blob.UploadFromStreamAsync(fs);
            }
            catch (Exception)
            {
                throw new AzureException(
                    "Failed to connect to Azure Blob from docker container! Please reboot docker and try again!");
            }
        }

        public async Task DeleteFileAsync(string path)
        {
            var container = AzureCloudHelpers.GetBlobContainer();
            var blob = container.GetBlockBlobReference(path);

            await blob.DeleteAsync();
        }

        public async Task CheckLateFilesAsync()
        {
            var lateFiles = await _unitOfWork.RemovedNodeRepository.GetLateFiles();

            foreach (var removedNode in lateFiles.ToArray())
            {
                foreach (var fileVersion in removedNode.Node.FileVersions.ToArray())
                    await DeleteFileAsync(fileVersion.PathToFile);

                _unitOfWork.NodeRepository.DeleteCascadeLateNode(removedNode.Node);
            }
        }
    }
}
