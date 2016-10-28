(function (angular) {
  'use strict';

  angular
      .module('app')
      .factory('userService', userService);

  userService.$inject = ['$http', '$q', 'spinnerService'];

  function userService($http, $q, spinnerService) {

    var service = {
      signUp: signUp,
      login: login
    };
    return service;


    function signUp(credentials) {
      spinnerService.showSpinner();
      return $http.post('/api/users', credentials).then(function (response) {
        spinnerService.hideSpinner();
        console.log(response);
        return response.data;
      }).catch(function (data) {
        spinnerService.hideSpinner();
        return $q.reject(data);
      });
    }

    function login(credentials) {
      spinnerService.showSpinner();
      return $http.post('/api/token', getPostTokenBodyForUser(credentials), {
        headers: { 'Content-Type': 'application/x-www-form-urlencoded; charset=UTF-8' }
      }).then(function (response) {
        console.log(response);
        var data = response.data;
        spinnerService.hideSpinner();
        return data;
      }).catch(function (data) {
        spinnerService.hideSpinner();
        return $q.reject(data);
      });
    }

    function getPostTokenBodyForUser(credentials) {
      return 'login=' + credentials.login + '&password=' + credentials.password;
    }
  }
})(angular);
