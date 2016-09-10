//locationsController.js
(function(){

    "use strict";

    angular.module("app-locations")
        .controller("locationsController", locationsController);

    function locationsController($http) {
        
        var vm = this;

        vm.locations = [];

        vm.errorMessage = "";
        vm.isBusy = true;

        vm.sortType = 'brewery.name';
        vm.sortReverse = false;
        vm.searchBeerName = '';

        $http.get("/api/locations")
            .then(
                //Success
                function (response) {
                    angular.copy(response.data, vm.locations);
                    
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