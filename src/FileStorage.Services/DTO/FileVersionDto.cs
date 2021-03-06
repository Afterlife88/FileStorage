﻿using System;

namespace FileStorage.Services.DTO
{
    public class FileVersionDto
    {
        public Guid? FileId { get; set; }
        public DateTime Created { get; set; }
        public string MD5Hash { get; set; }
        public int VersionOfFile { get; set; }
        public string UniqueFileName { get; set; }
        public long Size { get; set; }
    }
}
