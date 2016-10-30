using FileStorage.DAL.Contracts;
using FileStorage.Services.Contracts;
using FileStorage.Services.Implementation;
using Moq;

namespace FileStorage.Tests
{
    public static  class MockingManager
    {
        public static Mock<IUnitOfWork> GetFakeUnitOfWork()
        {
            return GetFakeUnitOfWork(MockBehavior.Loose);
        }

        public static Mock<IUnitOfWork> GetFakeUnitOfWork(MockBehavior mockBehavior)
        {
            return new Mock<IUnitOfWork>(mockBehavior);
        }

        public static IFileService GetFileService(IUnitOfWork unitOfWork, IBlobService blobService)
        {
            return new FileService(unitOfWork, blobService);
        }

        public static IFolderService GetFolderService(IUnitOfWork unitOfWork)
        {
            return new FolderService(unitOfWork);
        }
        public static ISearchService GetSearchService(IUnitOfWork unitOfWork)
        {
            return new SearchService(unitOfWork);
        }
        public static IBlobService GetBlobService(IUnitOfWork unitOfWork)
        {
            return new AzureBlobService(unitOfWork);
        }

    }
}
