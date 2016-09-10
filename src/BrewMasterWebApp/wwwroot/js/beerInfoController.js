//beerInfoController.js
(function () {
    "use strict";

    angular.module("app-beers")
        .controller("beerInfoController", beerInfoController);

    function beerInfoController($routeParams,$http) {
        
        var vm = this;

        vm.beerId = $routeParams.beerId;
        vm.beer = [];
        
        vm.errorMessage = "";
        vm.isBusy = true;

        var url = "/api/beer/" + vm.beerId;

        $http.get(url)
            .then(
                //Success
                function (response) {
                    angular.copy(response.data, vm.beer);
                },
                //Failure
                function(error){
                    vm.errorMessage = "Failed to load data for beer with id: " + vm.beerId;
                })
            .finally(function () {
                vm.isBusy = false;
            });
    }

})();