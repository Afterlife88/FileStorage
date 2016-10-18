using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using FileStorage.Contracts.Services;

namespace FileStorage.Cloud
{
    public class AzureBlobService : IBlobService
    {
        public Task<Stream> DownloadFileAsync(string path)
        {
            throw new NotImplementedException();
        }

        public Task<Stream> UploadFileAsync(HttpContent httpContent)
        {
            throw new NotImplementedException();
        }

        public Task DeleteFileAsync(string path)
        {
            throw new NotImplementedException();
        }
    }
}
