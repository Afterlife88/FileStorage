using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FileStorage.Web.DTO
{
    public class NodeDto
    {

        public int Id { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public DateTime Created { get; set; }
        public bool IsDirectory { get; set; }
        public int DirectoryId { get; set; }
        public string OwnerId { get; set; }
        public List<FileVersionDto> FileVersions { get; set; }
    }
}
