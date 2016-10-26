using System;
using System.Collections.Generic;

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
        public string DirectoryName { get; set; }
        public string OwnerId { get; set; }
        public List<FileVersionDto> FileVersions { get; set; }
    }
}
