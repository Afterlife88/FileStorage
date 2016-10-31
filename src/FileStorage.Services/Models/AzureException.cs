using System;

namespace FileStorage.Services.Models
{
    public class AzureException : Exception
    {
        public AzureException(string message) : base(message)
        {
            
        }
    }
}
