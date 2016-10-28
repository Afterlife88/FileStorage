(function () {
  'use strict';

  angular
      .module('app')
      .controller('fileVersionsController', fileVersionsController);

  fileVersionsController.$inject = ['data', '$uibModalInstance'];

  function fileVersionsController(data, $uibModalInstance) {
    this.data = data;
    this.cancel = cancel;

    function cancel() {
      $uibModalInstance.close();
    }
  }
})();
