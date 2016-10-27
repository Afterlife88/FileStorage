using System.Collections.Generic;

namespace FileStorage.Services.DTO
{
    public class WorkPlaceDto
    {
        public int FolderId { get; set; }
        public IList<FileDto> Items { get; set; }
    }
}
