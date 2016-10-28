using System;
using System.Collections.Generic;

namespace FileStorage.Services.DTO
{
    public class FileDto
    {
        public Guid UniqueFileId { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public DateTime Created { get; set; }
        public Guid DirectoryId { get; set; }
        public string DirectoryName { get; set; }
        public string OwnerId { get; set; }
        public List<FileVersionDto> FileVersions { get; set; }
    }
}
