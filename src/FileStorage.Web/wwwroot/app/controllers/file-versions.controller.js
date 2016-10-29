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
        console.log(response);
        var url = URL.createObjectURL(new Blob([response]));
        var a = document.createElement('a');
        a.href = url;
        a.download = fileName;
        a.target = '_blank';
        a.click();
      }).catch(function (err) {
        Alertify.error(err.data);
      });
    }

    //function getDowloandLink(parameters) {
    //  return pathToFiles + parameters.fileId + '?versionOfFile=' + parameters.versionOfFile;
    //}
    function cancel() {
      $uibModalInstance.close();
    }
  }
})();
