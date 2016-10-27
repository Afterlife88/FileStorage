using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FileStorage.Domain.Entities
{
    public class Node
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        [Required]
        public string Name { get; set; }
        // MIME mapping for the specified file name.
        public string ContentType { get; set; }
        public DateTime Created { get; set; }
        public bool IsDirectory { get; set; }
        public Guid? FolderId { get; set; }
        public virtual Node Folder{ get; set; }
        public bool IsDeleted { get; set; }
        public string OwnerId { get; set; }
        public virtual ApplicationUser Owner { get; set; }
        public int? PermissionId { get; set; }
        [ForeignKey("PermissionId")]
        public virtual Permission Permission { get; set; }
        public virtual ICollection<Node> Siblings { get; set; }
        public virtual ICollection<FileVersion> FileVersions { get; set; }
    }
}
