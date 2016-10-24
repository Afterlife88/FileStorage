using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace FileStorage.Domain.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public override string Email { get; set; }
    }
}
