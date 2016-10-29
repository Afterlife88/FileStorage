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
      .when('/files', {
        templateUrl: './app/views/file-storage.view.html',
        controller: 'fileStorageController',
        controllerAs: 'vm'
      })
      .when('/recycle-bin',
      {
        templateUrl: './app/views/recycle-bin.view.html',
        controller: 'recycleBinController',
        controllerAs: 'vm'
      })
      .when('/search',
      {
        templateUrl: './app/views/search.view.html',
        controller: 'searchController',
        controllerAs: 'vm'
      })
      .otherwise({
        redirectTo: "/files"
      });

    $locationProvider.hashPrefix('');

    $httpProvider.interceptors.push('authInterceptorService');
  }
]);