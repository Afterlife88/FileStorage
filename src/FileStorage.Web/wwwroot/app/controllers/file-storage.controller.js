(function () {
  'use strict';

  angular
      .module('app')
      .controller('fileStorageController', fileStorageController);

  fileStorageController.$inject = ['folderService', 'Alertify', 'FileUploader', '$uibModal', '$scope', 'Session'];

  function fileStorageController(folderService, Alertify, FileUploader, $uibModal, $scope, Session) {
    var vm = this;
    var modal = null;
    vm.changeFolder = changeFolder;
    vm.getFileVersions = getFileVersions;
    vm.addFolder = addFolder;

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

    function addFolder(folderId) {
      console.log(folderId);
      openAddFolderModal(folderId);
    }

    function getFileVersions(node) {
      //console.log(node);
      openFileVesrionsModal(node);
    }

    $scope.$on('folder-added', function (event, data) {
      changeFolder(data);
    });

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
        templateUrl: '/app/views/uploadFile.html',
        scope: $scope,
        backdrop: 'static'
      });

      modal.opened.then(function () {
        item.upload();
      });
      return modal;
    };


    function openAddFolderModal(folderId) {
      var addFolderModal = $uibModal.open({
        animation: true,
        templateUrl: '/app/views/addFolder.html',
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
        templateUrl: '/app/views/file-versions.html',
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
  }
})();
