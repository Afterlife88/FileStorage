(function (angular) {
  'use strict';

  angular
      .module('app')
      .factory('folderService', folderService);

  folderService.$inject = ['$http', '$q', 'spinnerService'];

  function folderService($http, $q, spinnerService) {

    var service = {
      getAllFolders: getAllFolders,
      getFolder: getFolder,
      addfolder: addfolder,
      getListOfFolders: getListOfFolders,
      replaceFolder: replaceFolder,
      renameFolder: renameFolder
    };
    return service;

    function getAllFolders() {
      spinnerService.showSpinner();
      return $http.get('/api/folders/tree-view')
        .then(function (response) {
          spinnerService.hideSpinner();

          return response.data;
        }).catch(function (data) {
          spinnerService.hideSpinner();
          return $q.reject(data);
        });
    }

    function getListOfFolders() {
      spinnerService.showSpinner();
      return $http.get('/api/folders')
        .then(function (response) {
          spinnerService.hideSpinner();

          return response.data;
        }).catch(function (data) {
          spinnerService.hideSpinner();
          return $q.reject(data);
        });
    }

    function getFolder(id) {
      spinnerService.showSpinner();
      return $http.get('/api/folders/' + id)
        .then(function (response) {
          spinnerService.hideSpinner();
          return response.data;
        })
        .catch(function (data) {
          spinnerService.hideSpinner();
          return $q.reject(data);
        });
    }

    function replaceFolder(folderId, destBody) {
      spinnerService.showSpinner();
      return $http.patch('/api/folders/replace/' + folderId, destBody)
      .then(function (response) {
        spinnerService.hideSpinner();
        return response.data;
      }).catch(function (data) {
        spinnerService.hideSpinner();
        return $q.reject(data);
      });
    }

    function renameFolder(folderId, renameBody) {
      spinnerService.showSpinner();
      return $http.patch('/api/folders/rename/' + folderId, renameBody).then(function (response) {
        spinnerService.hideSpinner();
        return response.data;
      }).catch(function (data) {
        spinnerService.hideSpinner();
        return $q.reject(data);
      });
    }
    function addfolder(data) {
      spinnerService.showSpinner();
      return $http.post('/api/folders/', data).then(function (response) {
        spinnerService.hideSpinner();
        return response.data;
      }).catch(function (err) {
        spinnerService.hideSpinner();
        return $q.reject(err);
      });
    }
  }
})(angular);
