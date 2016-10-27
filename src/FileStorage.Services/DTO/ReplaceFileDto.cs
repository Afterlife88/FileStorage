using System;
using System.ComponentModel.DataAnnotations;

namespace FileStorage.Services.DTO
{
    public class ReplaceFileDto
    {
        [Required]
        public Guid DestanationFolderId { get; set; }
    }
}
