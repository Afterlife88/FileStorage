(function () {
  'use strict';

  angular
      .module('app')
      .controller('indexController', indexController);

  indexController.$inject = ['$location', 'userService', 'Session'];

  function indexController($location, userService, Session) {
    var vm = this;
    vm.userName = Session.userName;
    vm.logout = logout;
    console.log(vm.userName);

    if (vm.userName === null)
      $location.path('/login');

    function logout() {
      Session.destroy();
      $location.path('/home');
    }
  }
})();
