(function () {
  'use strict';

  angular
      .module('app')
      .factory('userService', userService);

  userService.$inject = ['spinnerService'];

  function userService(spinnerService) {

    var service = {
      signUp: signUp,
      login: login
    };
    return service;

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
})();
