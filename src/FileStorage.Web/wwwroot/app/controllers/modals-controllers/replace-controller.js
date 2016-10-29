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
        console.log(vm.arrOfFolders);
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
          console.log(response);
          Alertify.success('File moved successfully!');
          $uibModalInstance.close();
          $scope.$emit('node-replaced', data.directoryId);
        }).catch(function (err) {
          Alertify.error(err.data);
        });
      } else {

      }

    }

    function cancel() {
      $uibModalInstance.close();
    }
  }
})();
