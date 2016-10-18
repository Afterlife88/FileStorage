using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace FileStorage.Utils
{
    public static class AzureCloudHelpers
    {
        public static CloudBlobContainer GetBlobContainer()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()).
                AddJsonFile("appconfig.json");

            var config = builder.Build();

            var blobStorageConnectionString = config.GetConnectionString("AzureBlobConnection");
            var blobStorageContainerName = config.GetConnectionString("AzureBlobContainerName");

            var blobStorageAccount = CloudStorageAccount.Parse(blobStorageConnectionString);
            var blobClient = blobStorageAccount.CreateCloudBlobClient();
            return blobClient.GetContainerReference(blobStorageContainerName);
        }
    }
}
