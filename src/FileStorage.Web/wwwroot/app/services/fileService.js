(function (angular) {
  'use strict';

  angular
      .module('app')
      .factory('fileService', fileService);

  fileService.$inject = ['$http', '$q', 'spinnerService'];

  function fileService($http, $q, spinnerService) {

    var service = {
      getConcreteVersion: getConcreteVersion
    };
    return service;

    function getConcreteVersion(fileId, vesrionId) {
      spinnerService.showSpinner();
      return $http.get('/api/files/' + fileId + '?versionOfFile=' + vesrionId,
        { responseType: 'arraybuffer' })
        .then(function (response) {
          spinnerService.hideSpinner();

          return response.data;
        }).catch(function (data) {
          spinnerService.hideSpinner();
          return $q.reject(data);
        });
    }
  }
})(angular);
