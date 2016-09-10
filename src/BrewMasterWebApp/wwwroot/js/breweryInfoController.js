//breweryInfoController.js
(function () {
    "use strict";

    angular.module("app-beers")
        .controller("breweryInfoController", breweryInfoController);

    function breweryInfoController($routeParams,$http) {
        
        var vm = this;

        vm.breweryId = $routeParams.breweryId;
        vm.brewery = [];
        
        vm.errorMessage = "";
        vm.isBusy = true;

        var url = "/api/brewery/" + vm.breweryId;

        $http.get(url)
            .then(
                //Success
                function (response) {
                    angular.copy(response.data, vm.brewery);
                },
                //Failure
                function(error){
                    vm.errorMessage = "Failed to load data for brewery with id: " + vm.breweryId;
                })
            .finally(function () {
                vm.isBusy = false;
            });
    }

})();