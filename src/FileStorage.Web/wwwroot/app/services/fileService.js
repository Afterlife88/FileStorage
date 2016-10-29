﻿(function (angular) {
  'use strict';

  angular
      .module('app')
      .factory('fileService', fileService);

  fileService.$inject = ['$http', '$q', 'spinnerService'];

  function fileService($http, $q, spinnerService) {

    var service = {
      getConcreteVersion: getConcreteVersion,
      renameFile: renameFile,
      downloadLatest: downloadLatest
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
    function renameFile(fileId, renameBody) {
      spinnerService.showSpinner();
      return $http.patch('/api/files/rename/' + fileId, renameBody).then(function (response) {
        spinnerService.hideSpinner();
        return response.data;
      }).catch(function (data) {
        spinnerService.hideSpinner();
        return $q.reject(data);
      });
    }
    function downloadLatest(fileId) {
      spinnerService.showSpinner();
      return $http.get('/api/files/' + fileId, { responseType: 'arraybuffer' }).then(function (response) {
        spinnerService.hideSpinner();
        return response.data;
      }).catch(function (data) {
        spinnerService.hideSpinner();
        return $q.reject(data);
      });
    }
  }
})(angular);
