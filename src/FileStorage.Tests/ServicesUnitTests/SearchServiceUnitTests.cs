using System.Collections.Generic;
using System.Threading.Tasks;
using FileStorage.DAL.Contracts;
using FileStorage.DAL.Contracts.Repositories;
using FileStorage.Web.Configuration;
using Moq;
using Xunit;

namespace FileStorage.Tests.ServicesUnitTests
{
    public class SearchServiceUnitTests
    {
        [Fact]
        public async Task GetAllPath_ReturnListOfPathDto_AfterMappingModelToDto()
        {

            var mockRepo = new Mock<INodeRepository>();


            var mockUnitOfWork = new Mock<IUnitOfWork>();
       
            //// Arrange
            //var mockRepo = new Mock<IUnitOfWork>();
            //mockRepo.Setup(repo => repo.GetAllAsync()).Returns(Task.FromResult(GetPathList()));
            //var service = new PathService(mockRepo.Object);
            //AutomapperConfiguration.Load();

            //// Act
            //var result = await service.GetAllExistedPathsAsync();

            //// Assert
            //var list = Assert.IsType<List<PathDto>>(result);
            Assert.Equal(4, list.Count);
        }

    
    }
}
