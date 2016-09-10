//locationInfoController.js
(function () {
    "use strict";

    angular.module("app-locations")
        .controller("locationInfoController", locationInfoController);

    function locationInfoController($routeParams, $http) {

        var vm = this;

        vm.locationId = $routeParams.locationId;
        vm.location = [];

        vm.errorMessage = "";
        vm.isBusy = true;

        var url = "/api/location/" + vm.locationId;

        $http.get(url)
            .then(
                //Success
                function (response) {
                    angular.copy(response.data, vm.location);
                },
                //Failure
                function (error) {
                    vm.errorMessage = "Failed to load data for location with id: " + vm.locationId;
                })
            .finally(function () {
                vm.isBusy = false;
            });
    }

})();