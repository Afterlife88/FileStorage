(function () {
  'use strict';

  angular
      .module('app')
      .controller('fileStorageController', fileStorageController);

  fileStorageController.$inject = ['folderService', 'Alertify', 'FileUploader', '$uibModal', '$scope', 'Session', 'fileService'];

  function fileStorageController(folderService, Alertify, FileUploader, $uibModal, $scope, Session, fileService) {
    var vm = this;
    var modal = null;
    vm.changeFolder = changeFolder;
    vm.getFileVersions = getFileVersions;
    vm.addFolder = addFolder;
    vm.downloadFile = downloadFile;
    vm.renameFile = renameFile;

    vm.workPlaceItems = {
      filesAndFolders: []
    };

    vm.uploader = new FileUploader({
      headers: { "Authorization": Session.accessToken },
      url: '/api/files/',
      removeAfterUpload: true
    });

    setupUploader(vm.uploader);
    activate();

    function activate() {
      return folderService.getAllFolders().then(function (response) {
        vm.workPlaceItems = response;
        vm.workPlaceItems.filesAndFolders = response.folders.concat(response.files);
      }).catch(function (err) {
        Alertify.error(err.data);
      });
    }
    function changeFolder(folderId) {
      return folderService.getFolder(folderId)
        .then(function (response) {
          vm.workPlaceItems = response;
          vm.workPlaceItems.filesAndFolders = response.folders.concat(response.files);
        }).catch(function (err) {
          Alertify.error(err.data);
        });
    }
    function downloadFile(file) {
      return fileService.downloadLatest(file.uniqueFileId).then(function (response) {
        var url = URL.createObjectURL(new Blob([response]));
        var a = document.createElement('a');
        a.href = url;
        a.download = file.name;
        a.target = '_blank';
        a.click();
      }).catch(function (err) {
        Alertify.error(err.data);
      });
    }

    function addFolder(folderId) {
      openAddFolderModal(folderId);
    }

    function getFileVersions(node) {
      openFileVesrionsModal(node);
    }
    function renameFile(file) {
      openRenameFileModal(file);
    }

    // Listeners
    $scope.$on('folder-added', function (event, data) {
      changeFolder(data);
    });
    $scope.$on('file-renamed', function (event, data) {
      changeFolder(data);
    });


    /// Helpers, modals etc

    function setupUploader(uploader) {
      uploader.onAfterAddingFile = function (item) {
        item.url = '/api/files/?directoryUniqId=' + vm.workPlaceItems.uniqueFolderId;
        modal = openProgressModal(item);
      };

      uploader.onSuccessItem = function () {
        setTimeout(function () {
          modal.close();
          $scope.progress = 0;
        }, 500);
        changeFolder(vm.workPlaceItems.uniqueFolderId);
      };

      uploader.onProgressItem = function (item, progress) {
        $scope.progress = progress;
      };
      uploader.onErrorItem = function (item, response) {
        setTimeout(function () {
          modal.close();
          $scope.progress = 0;
        }, 500);
        Alertify.error(response);
      };
    }

    function openProgressModal(item) {
      var modal = $uibModal.open({
        animation: true,
        templateUrl: '/app/views/modals/uploadFile.html',
        scope: $scope,
        backdrop: 'static'
      });

      modal.opened.then(function () {
        item.upload();
      });
      return modal;
    }

    function openAddFolderModal(folderId) {
      var addFolderModal = $uibModal.open({
        animation: true,
        templateUrl: '/app/views/modals/addFolder.html',
        backdrop: 'static',
        controller: 'addFolderController as vm',
        scope: $scope,
        resolve: {
          data: function () {
            return folderId;
          }
        }
      });
      return addFolderModal;
    }

    function openFileVesrionsModal(item) {
      var fileVersionsModal = $uibModal.open({
        animation: true,
        templateUrl: '/app/views/modals/file-versions.html',
        backdrop: 'static',
        controller: 'fileVersionsController as vm',
        size: 'lg',
        resolve: {
          data: function () {
            return item;
          }
        }
      });
      return fileVersionsModal;
    }

    function openRenameFileModal(item) {
      var renameFileModal = $uibModal.open({
        animation: true,
        templateUrl: '/app/views/modals/renameFile.html',
        backdrop: 'static',
        controller: 'renameFileController as vm',
        scope: $scope,
        resolve: {
          data: function () {
            return item;
          }
        }
      });
      return renameFileModal;
    }
  }
})();
