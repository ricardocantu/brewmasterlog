//breweryBeersController.js
(function () {
    "use strict";

    angular.module("app-brewery")
        .controller("breweryBeersController", breweryBeersController);

    function breweryBeersController($http) {
        
        var vm = this;
        vm.breweryId = "1mv7t1";
        vm.beers = [];

        vm.errorMessage = "";
        vm.isBusy = true;

        vm.sortType = 'name';
        vm.sortReverse = false;
        vm.searchBeerName = '';

        var url = "/api/beers/brewery/" + vm.breweryId;

        $http.get(url)
            .then(
                //Success
                function (response) {
                    angular.copy(response.data, vm.beers);
                    
                },
                //Failure
                function(error){
                    vm.errorMessage = "Failed to load data: " + error;
                })
            .finally(function () {
                vm.isBusy = false;
            });
    }

})();