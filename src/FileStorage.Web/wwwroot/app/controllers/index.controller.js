(function () {
  'use strict';

  angular
      .module('app')
      .controller('indexController', indexController);

  indexController.$inject = ['$location', 'userService', 'Session', '$rootScope'];

  function indexController($location, userService, Session, $rootScope) {
    var vm = this;
    vm.userName = Session.userName;
    $rootScope.isAuth = Session.isAuth;
    vm.logout = logout;
    console.log(vm.userName);

    if (!vm.isAuth) {
      Session.isAuth = false;
      $rootScope.isAuth = false;
      $location.path('/login');
    }
    function logout() {
      Session.isAuth = false;
      $rootScope.isAuth = false;
      Session.destroy();
      $location.path('/login');
    }
  }
})();
