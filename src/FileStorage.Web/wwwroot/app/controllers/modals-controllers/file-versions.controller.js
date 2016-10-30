(function () {
  'use strict';

  angular
      .module('app')
      .controller('fileVersionsController', fileVersionsController);

  fileVersionsController.$inject = ['data', '$uibModalInstance', 'fileService', 'Alertify'];

  function fileVersionsController(data, $uibModalInstance, fileService, Alertify) {
    this.data = data;
    this.cancel = cancel;
    this.downloadConcreteVersion = downloadConcreteVersion;

    function downloadConcreteVersion(fileName, item) {

      return fileService.getConcreteVersion(item.fileId, item.versionOfFile).then(function (response) {
        var url = URL.createObjectURL(new Blob([response]));
        var a = document.createElement('a');
        a.href = url;
        a.download = fileName;
        a.target = '_blank';
        document.body.appendChild(a);
        a.click();
      }).catch(function (err) {
        Alertify.error(err.data);
      });
    }

    function cancel() {
      $uibModalInstance.close();
    }
  }
})();
