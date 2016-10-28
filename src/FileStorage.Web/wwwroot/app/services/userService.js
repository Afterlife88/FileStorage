(function () {
    'use strict';

    angular
        .module('app')
        .factory('userService', userService);

    userService.$inject = ['$location'];

    function userService($location) {

      return 'item';
    }
})();
