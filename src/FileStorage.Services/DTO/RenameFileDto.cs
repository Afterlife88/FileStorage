using System.ComponentModel.DataAnnotations;

namespace FileStorage.Services.DTO
{
    public class RenameFileDto
    {
        [Required]
        public string NewName { get; set; }
    }
}
