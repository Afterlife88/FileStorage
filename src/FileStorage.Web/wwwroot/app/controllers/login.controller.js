(function () {
  'use strict';

  angular
      .module('app')
      .controller('loginController', loginController);

  loginController.$inject = ['$location', 'userService', 'Session'];

  function loginController($location, userService, Session) {
    var vm = this;
    vm.loginData = {};
    vm.login = login;
    vm.message = "";

    function login(data) {
      return userService.login(data).then(function (result) {
        console.log(result);
        var token = 'Bearer ' + result.access_token;
        Session.create(token, data.username);
        $location.path('/home');
       
      }).catch(function (err) {
        vm.created = false;
        vm.message = err.data;
      });
    }
  }
})();
