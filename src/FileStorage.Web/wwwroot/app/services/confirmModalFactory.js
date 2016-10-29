(function () {
    'use strict';

    angular
    .module('app')
    .factory("confirmModalFactory", confirmModalFactory);

    //confirmModalFactory.$inject = ['$scope', '$uibModalInstance'];

    function confirmModalFactory() {
      return {
        createModalController: function (header, message) {
          var modalInstanceCtrl = function ($scope, $uibModalInstance) {
            $scope.header = header;
            $scope.message = message;
            $scope.yes = function () {
              $uibModalInstance.close(true);
            };
            $scope.no = function () {
              $uibModalInstance.close(false);
            };
          };

          return modalInstanceCtrl;
        }
      }
    };

})();
