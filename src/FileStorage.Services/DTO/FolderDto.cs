using System;
using System.Collections.Generic;

namespace FileStorage.Services.DTO
{
    public class FolderDto
    {
        public Guid UniqueFolderId { get; set; }
        public DateTime Created { get; set; }
        public string FolderName { get; set; }
        public string ParentFolderName { get; set; }
        public string ParentFolderId { get; set; }
        public string OwnerId { get; set; }
        public List<FileDto> Files { get; set; }
        public List<FolderDto> Folders { get; set; }
    }
}
