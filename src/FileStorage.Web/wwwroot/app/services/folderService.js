(function (angular) {
  'use strict';

  angular
      .module('app')
      .factory('folderService', folderService);

  folderService.$inject = ['$http', '$q', 'spinnerService'];

  function folderService($http, $q, spinnerService) {

    var service = {
      getAllFolders: getAllFolders
    };
    return service;

    function getAllFolders() {
      spinnerService.showSpinner();

      return $http.get('/api/folders')
        .then(function(response) {
          spinnerService.hideSpinner();

          return response.data;
        }).catch(function(data) {
          spinnerService.hideSpinner();
          return $q.reject(data);
        });
    }
    
  }
})(angular);
