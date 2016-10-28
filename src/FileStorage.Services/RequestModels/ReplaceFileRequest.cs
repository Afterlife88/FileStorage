using System;
using System.ComponentModel.DataAnnotations;

namespace FileStorage.Services.RequestModels
{
    public class ReplaceFileRequest
    {
        [Required]
        public Guid DestanationFolderId { get; set; }
    }
}
