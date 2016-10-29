(function () {
  'use strict';

  angular
      .module('app')
      .controller('renameFolderController', renameFolderController);

  renameFolderController.$inject = ['data', 'folderService', 'Alertify', '$uibModalInstance', '$scope'];

  function renameFolderController(data, folderService, Alertify, $uibModalInstance, $scope) {
    var vm = this;
    vm.data = data;
    vm.name = data.name;
    vm.cancel = cancel;
    vm.ok = ok;


    function ok(name) {
      var request = {
        newName: name
      };
      return folderService.renameFolder(vm.data.uniqueFolderId, request).then(function () {
        Alertify.success('Folder renamed successfully!');
        $uibModalInstance.close();
        $scope.$emit('updateFolder', vm.data.parentFolderId);
      }).catch(function (err) {
        Alertify.error(err.data);
      });
    }

    function cancel() {
      $uibModalInstance.close();
    }
  }
})();
