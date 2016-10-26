using System.Collections.Generic;

namespace FileStorage.Services.DTO
{
    public class WorkPlaceDto
    {
        public int FolderId { get; set; }
        public IList<NodeDto> Items { get; set; }
    }
}
