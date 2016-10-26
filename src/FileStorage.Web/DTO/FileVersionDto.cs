using System;

namespace FileStorage.Web.DTO
{
    public class FileVersionDto
    {
        public int Id { get; set; }
        public int FileId { get; set; }
        public DateTime Created { get; set; }
        public string MD5Hash { get; set; }
        public string UniqueFileName { get; set; }
        public long Size { get; set; }
    }
}
