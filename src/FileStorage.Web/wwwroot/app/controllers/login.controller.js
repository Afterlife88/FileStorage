(function () {
  'use strict';

  angular
      .module('app')
      .controller('loginController', loginController);

  loginController.$inject = ['$location', 'userService'];

  function loginController($location, userService) {
    /* jshint validthis:true */
    var vm = this;
    vm.title = 'index';

    activate();

    function activate() { }
  }
})();
