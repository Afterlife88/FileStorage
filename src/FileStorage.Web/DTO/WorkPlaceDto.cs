using System.Collections.Generic;

namespace FileStorage.Web.DTO
{
    public class WorkPlaceDto
    {
        public int FolderId { get; set; }
        public IList<NodeDto> Items { get; set; }
    }
}
