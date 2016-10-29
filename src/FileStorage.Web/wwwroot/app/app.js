(function (angular) {
  'use strict';

  // Angular module for the application
  angular.module('app', [
    'ngRoute',
    'Alertify',
    'angularSpinner',
    'angularFileUpload',
    'ui.bootstrap'
  ]);

  angular.module('app').run(['Session', '$rootScope',
    function (Session, $rootScope) {
      Session.fillAuthData();
      $rootScope.isAuth = Session.isAuth;
    }
  ]);
  angular.module('app').constant('pathToFiles', "/api/files/");
})(angular);