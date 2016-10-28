(function () {
  'use strict';

  angular
      .module('app')
      .controller('fileStorageController', fileStorageController);

  fileStorageController.$inject = ['$location'];

  function fileStorageController($location) {
    /* jshint validthis:true */
    var vm = this;
    vm.title = 'index';

    activate();

    function activate() { }
  }
})();
