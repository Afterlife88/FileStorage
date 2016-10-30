using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FileStorage.DAL.Contracts;
using FileStorage.DAL.Contracts.Repositories;
using FileStorage.Domain.Entities;
using FileStorage.Web.Configuration;
using Moq;
using Xunit;

namespace FileStorage.Tests.ServicesUnitTests
{
    public class SearchServiceUnitTests
    {
        [Fact]
        public async Task Search_Returns_Non_Deleted_Files_Or_Folders_Successful()
        {

            var fakeUnitOfWork = MockingManager.GetFakeUnitOfWork();

            var node1 = new Node()
            {
                IsDirectory = false,
                ContentType = "application/json",
                Name = "name",
                IsDeleted = false,
                OwnerId = "qweqweqwe-123123-asdasdxzc-asdasdasd",
                Created = DateTime.Now,
                FolderId = new Guid(),
                

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

            var result = await searchService.SearchFilesAsync("test@gmail.com", "name", false);



            //// Arrange
            //var mockRepo = new Mock<IUnitOfWork>();
            //mockRepo.Setup(repo => repo.GetAllAsync()).Returns(Task.FromResult(GetPathList()));
            //var service = new PathService(mockRepo.Object);
            //AutomapperConfiguration.Load();

            //// Act
            //var result = await service.GetAllExistedPathsAsync();

            //// Assert
            //var list = Assert.IsType<List<PathDto>>(result);
            Assert.Equal(1, result.Count());
        }


    }
}
