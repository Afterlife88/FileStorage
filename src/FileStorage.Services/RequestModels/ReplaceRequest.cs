using System;
using System.ComponentModel.DataAnnotations;

namespace FileStorage.Services.RequestModels
{
    public class ReplaceRequest
    {
        [Required]
        public Guid DestanationFolderId { get; set; }
    }
}
