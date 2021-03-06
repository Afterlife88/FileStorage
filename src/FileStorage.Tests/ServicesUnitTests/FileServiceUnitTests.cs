﻿using System;
using System.Linq;
using System.Threading.Tasks;
using FileStorage.Services.Models;
using FileStorage.Services.RequestModels;
using FileStorage.Tests.Helpers;
using Moq;
using Xunit;

namespace FileStorage.Tests.ServicesUnitTests
{
    public class FileServiceUnitTests
    {
        private string fakeEmail = "test@gmail.com";
        [Fact]
        public async Task GetUserFile_WhenRequestedFromAnonymousReturnError_Successful()
        {
            // Arrange
            var fakeUnitOfWork = MockingManager.GetFakeUnitOfWork();
            var fakeBlobService = MockingManager.GetBlobService(fakeUnitOfWork.Object);
            var fileService = MockingManager.GetFileService(fakeUnitOfWork.Object, fakeBlobService);

            // Act
            var result = await fileService.GetFileAsync(new Guid(), fakeEmail, 1);

            // Assert
            Assert.Equal(fileService.State.TypeOfError, TypeOfServiceError.Forbidden);
        }

        [Fact]
        public async Task GetUserFile_WhenRequestedFileNotExistReturnNotFound_Successful()
        {
            var nodes = TestData.CreateFiles();

            // Arrange
            var fakeUnitOfWork = MockingManager.GetFakeUnitOfWork();


            fakeUnitOfWork.Setup(t => t.NodeRepository.GetAllNodesForUserWithPredicate(It.IsAny<string>(), false)).ReturnsAsync(nodes);

            var fakeBlobService = MockingManager.GetBlobService(fakeUnitOfWork.Object);
            var fileService = MockingManager.GetFileService(fakeUnitOfWork.Object, fakeBlobService);

            // Act
            var result = await fileService.GetFileAsync(new Guid(), fakeEmail, 1);

            // Assert
            Assert.Equal(fileService.State.TypeOfError, TypeOfServiceError.NotFound);
            Assert.NotNull(result.Item2);
        }
        [Fact]
        public async Task GetUserFile_WhenRequestedExistedFileReturnFileStream_Successful()
        {
            var nodes = TestData.CreateFiles();

            // Arrange
            var fakeUnitOfWork = MockingManager.GetFakeUnitOfWork();


            fakeUnitOfWork.Setup(t => t.NodeRepository.GetAllNodesForUserWithPredicate(It.IsAny<string>(), false)).ReturnsAsync(nodes);

            var fakeBlobService = MockingManager.GetBlobService(fakeUnitOfWork.Object);
            var fileService = MockingManager.GetFileService(fakeUnitOfWork.Object, fakeBlobService);
            // Act
            var result = await fileService.GetFileAsync(new Guid("37e32a9e-bd72-48e2-9a7b-5c4fdbda6be1"), fakeEmail, 1);
            // Assert
            Assert.NotNull(result.Item1);
        }
        [Fact]
        public async Task RenameFile_ReturnOkWhenFileRenamed_Successful()
        {
            var nodes = TestData.CreateFiles();

            // Arrange
            var fakeUnitOfWork = MockingManager.GetFakeUnitOfWork();


            fakeUnitOfWork.Setup(t => t.NodeRepository.GetAllNodesForUserWithPredicate(It.IsAny<string>(), false)).ReturnsAsync(nodes);

            var fakeBlobService = MockingManager.GetBlobService(fakeUnitOfWork.Object);
            var fileService = MockingManager.GetFileService(fakeUnitOfWork.Object, fakeBlobService);

            // Act
            var result = await fileService.RenameFileAsync(new Guid("37e32a9e-bd72-48e2-9a7b-5c4fdbda6be1"), "new-name", fakeEmail);
            // Assert
            Assert.Equal(result.Name, "new-name");
        }
        [Fact]
        public async Task RestoreFile_RestoringRemovedFile_Successful()
        {
            var nodes = TestData.CreateFiles();

            // Arrange
            var fakeUnitOfWork = MockingManager.GetFakeUnitOfWork();


            fakeUnitOfWork.Setup(t => t.NodeRepository.GetAllNodesForUserWithPredicate(It.IsAny<string>(), false)).ReturnsAsync(nodes);

            var fakeBlobService = MockingManager.GetBlobService(fakeUnitOfWork.Object);
            var fileService = MockingManager.GetFileService(fakeUnitOfWork.Object, fakeBlobService);

            // Act 

            // Remove
            var removeRequest = await fileService.RemoveFileAsync(new Guid("37e32a9e-bd72-48e2-9a7b-5c4fdbda6be1"), fakeEmail);

            Assert.Equal(fileService.State.TypeOfError, TypeOfServiceError.Success);
            // Restore
            var restoreRequest = await fileService.RestoreRemovedFileAsync(new Guid("37e32a9e-bd72-48e2-9a7b-5c4fdbda6be1"),
                fakeEmail);

            // Assert 
            Assert.NotNull(restoreRequest);
        }


        public async Task ReplaceFile_ReplacingFileAndPlacingInRequestedFolder_Successful()
        {

            var nodes = TestData.CreateFiles();


            // Arrange
            var fakeUnitOfWork = MockingManager.GetFakeUnitOfWork();


            fakeUnitOfWork.Setup(t => t.NodeRepository.GetAllNodesForUserWithPredicate(It.IsAny<string>(), false)).ReturnsAsync(nodes);

            var fakeBlobService = MockingManager.GetBlobService(fakeUnitOfWork.Object);
            var fileService = MockingManager.GetFileService(fakeUnitOfWork.Object, fakeBlobService);

            // Act 
            var replaceRequest = await fileService.ReplaceFileAsync(fakeEmail, new Guid("37e32a9e-bd72-48e2-9a7b-5c4fdbda6be1"),
                new ReplaceRequest() { DestanationFolderId = new Guid("37e32a9e-bd72-48e2-9a7b-5c4fdbda3xj5"), });


            // Assert 
            Assert.Equal(fileService.State.TypeOfError, TypeOfServiceError.Success);
            Assert.Equal(replaceRequest.DirectoryId, new Guid("37e32a9e-bd72-48e2-9a7b-5c4fdbda3xj5"));
        }
    }
}
