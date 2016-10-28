(function () {
  'use strict';

  angular
      .module('app')
      .controller('fileStorageController', fileStorageController);

  fileStorageController.$inject = ['folderService', 'Alertify'];

  function fileStorageController(folderService, Alertify) {
    var vm = this;
    vm.initData = {
      filesAndFolders: []
    };
    activate();

    function activate() {
      return folderService.getAllFolders().then(function (response) {
        vm.initData = response;
        vm.initData.filesAndFolders = response.folders.concat(response.files);
        console.log(vm.initData);
      }).catch(function (err) {
        Alertify.error(err.data);
      });
    }
  }
})();
