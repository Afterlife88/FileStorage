(function () {
  'use strict';

  angular
      .module('app')
      .controller('replaceNodeController', replaceNodeController);

  replaceNodeController.$inject = ['data', '$uibModalInstance', 'folderService', 'Alertify', '$scope', 'fileService'];

  function replaceNodeController(data, $uibModalInstance, folderService, Alertify, $scope, fileService) {

    var vm = this;
    vm.data = data;
    vm.arrOfFolders = [];
    vm.directoryToMove = {};
    vm.cancel = cancel;
    vm.ok = ok;
    activate();

    function activate() {
      return folderService.getListOfFolders().then(function (response) {
        vm.arrOfFolders = response;
        if (vm.data.uniqueFolderId) {
          var filteredArrayFromCallerFolder = vm.arrOfFolders.filter(function (el) {
            return el.uniqueFolderId !== vm.data.uniqueFolderId && el.parentFolderId !== vm.data.uniqueFolderId;
          });
          console.log(filteredArrayFromCallerFolder);
          vm.arrOfFolders = filteredArrayFromCallerFolder;
        }
      }).catch(function (err) {
        Alertify.error(err.data);
      });
    }


    function ok(nodeData, directoryToMove) {
      if (nodeData.contentType) {
        console.log(nodeData);
        var request = {
          destanationFolderId: directoryToMove.uniqueFolderId
        }
        return fileService.replaceFile(nodeData.uniqueFileId, request).then(function (response) {
          Alertify.success('File moved successfully!');
          $uibModalInstance.close();
          $scope.$emit('updateFolder', data.directoryId);
        }).catch(function (err) {
          Alertify.error(err.data);
        });
      } else {
        var folderChangeRequest = {
          destanationFolderId: directoryToMove.uniqueFolderId
        };
        return folderService.replaceFolder(nodeData.uniqueFolderId, folderChangeRequest).then(function (response) {
          Alertify.success('File moved successfully!');
          $uibModalInstance.close();
          $scope.$emit('updateFolder', data.parentFolderId);
        }).catch(function (err) {
          Alertify.error(err.data);
        });
      }
    }

    function cancel() {
      $uibModalInstance.close();
    }
  }
})();
