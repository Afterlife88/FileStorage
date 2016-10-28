angular.module('app').config(['$routeProvider', '$locationProvider', '$httpProvider',
  function ($routeProdiver, $locationProvider, $httpProvider) {
    $routeProdiver
      .when('/login', {
        templateUrl: './app/views/login.view.html',
        controller: 'loginController',
        controllerAs: 'vm'
      })
      .when('/signup', {
        templateUrl: './app/views/signup.view.html',
        controller: 'signupController',
        controllerAs: 'vm'
      })
      .when('/home', {
        templateUrl: './app/views/file-storage.view.html',
        controller: 'fileStorageController',
        controllerAs: 'vm'
      })
      .otherwise({
        redirectTo: "/home"
      });

    $locationProvider.hashPrefix('');

    $httpProvider.interceptors.push('authInterceptorService');
  }
]);