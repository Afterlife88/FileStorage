using System;
using System.Linq;
using System.Threading.Tasks;
using FileStorage.Domain.Entities;
using Moq;
using Xunit;
using Xunit.Abstractions;

namespace FileStorage.Tests.ServicesUnitTests
{
    public class SearchServiceUnitTests
    {
        private readonly ITestOutputHelper output;

        public SearchServiceUnitTests(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact]
        public async Task Search_Returns_Non_Deleted_Files_Or_Folders_Successful()
        {
            // Arrange
            var fakeUnitOfWork = MockingManager.GetFakeUnitOfWork();
       
            var node1 = new Node()
            {
                IsDirectory = false,
                ContentType = "application/json",
                Name = "name",
                IsDeleted = false,
                OwnerId = new Guid().ToString(),
                Created = DateTime.Now,
                FolderId = null,
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

            fakeUnitOfWork.Setup(t => t.NodeRepository.GetFolderByIdAsync(It.IsAny<Guid>())).ReturnsAsync(node1);
     
            var searchService = MockingManager.GetSearchService(fakeUnitOfWork.Object);

            fakeUnitOfWork.VerifyAll();
            // Act
            var result = await searchService.SearchFilesAsync("test@gmail.com", "name", false);

            // Assert
            Assert.Equal(1, result.Count());
        }


    }
}
