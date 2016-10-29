(function () {
  'use strict';

  angular
      .module('app')
      .controller('renameFileController', renameFileController);

  renameFileController.$inject = ['data', '$uibModalInstance', 'fileService', 'Alertify', '$scope'];

  function renameFileController(data, $uibModalInstance, fileService, Alertify, $scope) {
    this.data = data;
    this.name = data.name;
    this.cancel = cancel;
    this.ok = ok;


    function ok(name) {
      var request = {
        newName: name
      };
      return fileService.renameFile(data.uniqueFileId, request).then(function () {
        Alertify.success('File renamed successfully!');
        $uibModalInstance.close();
        $scope.$emit('updateFolder', data.directoryId);
      }).catch(function (err) {
        Alertify.error(err.data);
      });
    }

    function cancel() {
      $uibModalInstance.close();
    }
  }
})();
