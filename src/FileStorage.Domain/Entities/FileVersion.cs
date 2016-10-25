using System;

namespace FileStorage.Domain.Entities
{
    public class FileVersion
    {
        public int Id { get; set; }
        public int? NodeId { get; set; }
        public virtual Node Node { get; set; }
        public DateTime Created { get; set; }
        public string MD5Hash { get; set; }
        public string PathToFile { get; set; }
        public long Size { get; set; }
    }
}
