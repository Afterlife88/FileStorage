(function () {
  'use strict';

  angular
      .module('app')
      .controller('searchController', searchController);

  searchController.$inject = ['searchService', 'Alertify', 'fileService'];

  function searchController(searchService, Alertify, fileService) {
    var vm = this;
    vm.result = [];
    vm.query = null;
    vm.search = search;
    vm.includeDeleted = null;
    vm.showPropForRemoved = false;
    vm.download = download;

    function search(query, includeDeleted) {
      vm.result = [];
      console.log(query, includeDeleted);
      return searchService.search(query, includeDeleted).then(function (response) {
        console.log(response);
        if (includeDeleted)
          vm.showPropForRemoved = true;
        else
          vm.showPropForRemoved = false;
        vm.result = response;
      }).catch(function (err) {
        Alertify.error(err.data);
      });
    }

    function download(fileName, fileId) {
      return fileService.downloadLatest(fileId).then(function (response) {
        var url = URL.createObjectURL(new Blob([response]));
        var a = document.createElement('a');
        a.href = url;
        a.download = fileName;
        a.target = '_blank';
        a.click();
      }).catch(function (err) {
        Alertify.error(err.data);
      });
    }
  }
})();
