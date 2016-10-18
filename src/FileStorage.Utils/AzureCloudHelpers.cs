using FileStorage.Web;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace FileStorage.Utils
{
    public static class AzureCloudHelpers
    {
        public static CloudBlobContainer GetBlobContainer()
        {
            var blobStorageConnectionString = Startup.Configuration.GetConnectionString("AzureBlobConnection");
            var blobStorageContainerName = Startup.Configuration.GetConnectionString("AzureBlobContainerName");

            var blobStorageAccount = CloudStorageAccount.Parse(blobStorageConnectionString);
            var blobClient = blobStorageAccount.CreateCloudBlobClient();
            return blobClient.GetContainerReference(blobStorageContainerName);
        }
    }
}
