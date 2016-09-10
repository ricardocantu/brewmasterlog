//beersController.js
(function () {
    "use strict";

    angular.module("app-beers")
        .controller("beersController", beersController);

    function beersController($http) {
        
        var vm = this;

        vm.beers = [];

        vm.errorMessage = "";
        vm.isBusy = true;

        vm.sortType = 'name';
        vm.sortReverse = false;
        vm.searchBeerName = '';

        $http.get("/api/beers")
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