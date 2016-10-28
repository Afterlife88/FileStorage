using System.ComponentModel.DataAnnotations;

namespace FileStorage.Services.RequestModels
{
    public class RenameFileRequest
    {
        [Required]
        public string NewName { get; set; }
    }
}
