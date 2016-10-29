(function () {
  'use strict';

  angular
      .module('app')
      .controller('recycleBinController', recycleBinController);

  recycleBinController.$inject = ['searchService', 'Alertify', 'fileService'];

  function recycleBinController(searchService, Alertify, fileService) {
    var vm = this;
    vm.items = [];
    vm.restoreNode = restoreNode;

    init();

    function init() {
      return searchService.search(null, true)
        .then(function (response) {
          vm.items = response;
        }).catch(function (err) {
          Alertify.error(err.data);
        });
    }

    function restoreNode(node) {
      if (node.contentType) {
        return fileService.restoreFile(node.id).then(function () {
          Alertify.success('File restored successfully!');
          init();
        }).catch(function (err) {
          Alertify.error(err.data);
        });
      } else {
        //var folderChangeRequest = {
        //  destanationFolderId: directoryToMove.uniqueFolderId
        //};
        //return folderService.replaceFolder(node.uniqueFolderId, folderChangeRequest).then(function (response) {
        //  Alertify.success('Folder moved successfully!');
        //  $uibModalInstance.close();
        //  $scope.$emit('updateFolder', data.parentFolderId);
        //}).catch(function (err) {
        //  Alertify.error(err.data);
        //});
      }
    }
  }
})();
