using System.ComponentModel.DataAnnotations;

namespace FileStorage.Services.RequestModels
{
    public class RenameRequest
    {
        [Required]
        public string NewName { get; set; }
    }
}
