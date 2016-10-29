(function () {
  'use strict';

  angular
      .module('app')
      .controller('replaceNodeController', replaceNodeController);

  replaceNodeController.$inject = ['data', '$uibModalInstance', 'folderService', 'Alertify', '$scope', 'fileService', 'filterFilter'];

  function replaceNodeController(data, $uibModalInstance, folderService, Alertify, $scope, fileService, filterFilter) {

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
            return el.uniqueFolderId !== vm.data.uniqueFolderId;
          });
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
          $scope.$emit('node-replaced', data.directoryId);
        }).catch(function (err) {
          Alertify.error(err.data);
        });
      } else {
        console.log(nodeData, directoryToMove);
        console.log('folder')
      }

    }

    function cancel() {
      $uibModalInstance.close();
    }
  }
})();
