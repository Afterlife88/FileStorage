﻿using System;
using System.ComponentModel.DataAnnotations;

namespace FileStorage.Services.RequestModels
{
    public class CreateFolderRequest
    {
        /// <summary>
        /// Identificator for parent folder, can be null, when value not passed - folder will be created at root user folder
        /// </summary>
        public Guid? ParentFolderId { get; set; }
        /// <summary>
        /// Folder name
        /// </summary>
        [Required]
        public string Name { get; set; }
    }
}
