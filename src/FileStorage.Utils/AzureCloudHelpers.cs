using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace FileStorage.Utils
{
    public static class AzureCloudHelpers
    {
        public static CloudBlobContainer GetBlobContainer()
        {

            var blobStorageConnectionString = "DefaultEndpointsProtocol=https;AccountName=away4sandboxstorage;AccountKey=CwypfwoBWvKcRl3nvrHluMKT0o4nQGhbMETjr7lseNyu/O8vcd9p+7ewdyB32ZQSiMeTVDiMyU+AXtL8w+nP3Q==;EndpointSuffix=core.windows.net";
            var blobStorageContainerName =  "files";

            var blobStorageAccount = CloudStorageAccount.Parse(blobStorageConnectionString);
            var blobClient = blobStorageAccount.CreateCloudBlobClient();
            
            return blobClient.GetContainerReference(blobStorageContainerName);
        }
    }
}
