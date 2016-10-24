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

            var blobStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=devchallenge;AccountKey=p/p10wLXwJnGfZVo78fz9OrjXi4GgtxWQjuy2ApNGkgCdy3QO1ryTddqP88u4zb0VeaBgdMCyk8GGsUtYcqgrA==";
            var blobStorageContainerName =  "files";

            var blobStorageAccount = CloudStorageAccount.Parse(blobStorageConnectionString);
            var blobClient = blobStorageAccount.CreateCloudBlobClient();
            
            return blobClient.GetContainerReference(blobStorageContainerName);
        }
    }
}
