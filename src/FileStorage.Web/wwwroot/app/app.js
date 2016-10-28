(function (angular) {
  'use strict';

  // Angular module for the application
  angular.module('app', [
    'ngRoute',
    'Alertify',
    'angularSpinner'
  ]);

  angular.module('app').run(['$rootScope',
    function ($rootScope) {
      $rootScope.somevalue = 'adasda';
    }
  ]);
})(angular);