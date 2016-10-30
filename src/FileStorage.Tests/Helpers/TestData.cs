using System;
using System.Collections.Generic;
using FileStorage.Domain.Entities;

namespace FileStorage.Tests.Helpers
{
    public static class TestData
    {
        public static IEnumerable<Node> CreateFiles()
        {
            var node1 = new Node()
            {
                IsDirectory = false,
                ContentType = "application/json",
                Name = "name",
                IsDeleted = false,

            };
            var fileVersion = new FileVersion()
            {

                Created = DateTime.Today,
                MD5Hash = "111-222-333-444",
                PathToFile = "path_to_azure_file",
                Size = 1151651561,
                Node = node1,
                VersionOfFile = 1
            };
            node1.FileVersions.Add(fileVersion);
            yield return node1;

            var node2 = new Node()
            {
                IsDirectory = false,
                ContentType = "application/json",
                Name = "name",
                IsDeleted = false,

            };
            var fileVersion2 = new FileVersion()
            {

                Created = DateTime.Today,
                MD5Hash = "111-222-333-444",
                PathToFile = "path_to_azure_file",
                Size = 1151651561,
                Node = node1,
                VersionOfFile = 1
            };
            node2.FileVersions.Add(fileVersion2);
            yield return node2;

        }
    }
}
