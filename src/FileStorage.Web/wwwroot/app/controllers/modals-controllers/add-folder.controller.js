(function () {
  'use strict';

  angular
      .module('app')
      .controller('addFolderController', addFolderController);

  addFolderController.$inject = ['data', '$uibModalInstance', 'folderService', 'Alertify', '$scope'];

  function addFolderController(data, $uibModalInstance, folderService, Alertify, $scope) {
    this.data = data;
    this.name = "";
    this.cancel = cancel;
    this.ok = ok;


    function ok(name) {
      var request = {
        parentFolderId: data,
        name: name
      };
      return folderService.addfolder(request).then(function () {
        Alertify.success('Folder added successfully!');
        $uibModalInstance.close();
        $scope.$emit('updateFolder', request.parentFolderId);

      }).catch(function (err) {
        Alertify.error(err.data);
      });
    }

    function cancel() {
      $uibModalInstance.close();
    }
  }
})();
