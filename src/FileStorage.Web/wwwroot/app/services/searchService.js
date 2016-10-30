(function (angular) {
  'use strict';

  angular
      .module('app')
      .factory('searchService', searchService);

  searchService.$inject = ['$http', '$q', 'spinnerService'];

  function searchService($http, $q, spinnerService) {

    var service = {
      search: search
    };
    return service;

    function search(query, searchRemoved) {
      console.log(searchRemoved);
      var request = getRequest(query, searchRemoved);
      spinnerService.showSpinner();
      return $http.get('/api/search?query=' + request.query + '&includeRemoved=' + request.search).then(function (response) {
        spinnerService.hideSpinner();
        return response.data;
      }).catch(function (data) {
        spinnerService.hideSpinner();
        return $q.reject(data);
      });
    }
    function isEmpty(value) {
      return (typeof value === "undefined" || value === null);
    }


    function getRequest(query, searchRemoved) {
      var request = {
        query: '',
        search: ''
      }
      if (isEmpty(query)) {
        request.query = '';
      } else {
        request.query = query;
      }

      request.search = searchRemoved;
      return request;
    }
  }
})(angular);
