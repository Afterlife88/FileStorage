using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace FileStorage.Services.DTO
{
    public class SearchResultDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string ContentType { get; set; }
        public  DateTime Created { get; set; }
        public bool IsDirectory { get; set; }
        public bool IsDeleted { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? WillBeRemovedAt { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public DateTime? RemovedOn { get; set; }
    }
}
