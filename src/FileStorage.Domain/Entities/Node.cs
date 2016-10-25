using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace FileStorage.Domain.Entities
{
    public class Node
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public DateTime Created { get; set; }
        public string PathToFile { get; set; }
        public long Size { get; set; }
        public bool IsDirectory { get; set; }
        public int? RootId { get; set; }
        public virtual Node Root { get; set; }
        public string OwnerId { get; set; }
        public virtual ApplicationUser Owner { get; set; }
        public int? PermissionId { get; set; }
        [ForeignKey("PermissionId")]
        public virtual Permission Permission { get; set; }
        public virtual ICollection<Node> Siblings { get; set; }
    }
}
