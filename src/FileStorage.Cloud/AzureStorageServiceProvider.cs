using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace FileStorage.Cloud
{
    public class AzureStorageServiceProvider : MultipartFormDataContent
    {
        public AzureStorageServiceProvider() : base(Path.GetTempPath())
        { }

        
    }
}
