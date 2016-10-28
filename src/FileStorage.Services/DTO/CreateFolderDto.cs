using System;
using System.ComponentModel.DataAnnotations;

namespace FileStorage.Services.DTO
{
    public class CreateFolderDto
    {
        public Guid? ParentFolderId { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
