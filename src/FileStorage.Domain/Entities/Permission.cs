using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using FileStorage.Domain.Entities.Enums;

namespace FileStorage.Domain.Entities
{
    public class Permission
    {
        [Key]
        public int Id { get; set; }
        public AccessType AccessType { get; set; }
        public virtual ICollection<ShareEmail> AuthorizedUserEmails { get; set; }
    }
}
