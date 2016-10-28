(function () {
  'use strict';

  angular
      .module('app')
      .controller('fileStorageController', fileStorageController);

  fileStorageController.$inject = ['folderService', 'Alertify'];

  function fileStorageController(folderService, Alertify) {
    var vm = this;
    vm.workPlaceItems = {
      filesAndFolders: []
    };
    vm.changeFolder = changeFolder;
    vm.currentFolder = {};
    activate();

    function activate() {
      return folderService.getAllFolders().then(function (response) {
        vm.currentFolder = response;
        vm.workPlaceItems = response;
        vm.workPlaceItems.filesAndFolders = response.folders.concat(response.files);
      }).catch(function (err) {
        Alertify.error(err.data);
      });
    }

    function changeFolder(folderId) {

      return folderService.getFolder(folderId)
        .then(function (response) {
          console.log(response);
          vm.currentFolder = response;
          vm.workPlaceItems = response;
          vm.workPlaceItems.filesAndFolders = response.folders.concat(response.files);
        }).catch(function (err) {
          Alertify.error(err.data);
        });
    }
  }
})();
