//app-beers.js

(function () {
    
    "use strict";

    angular.module("app-beers", ["simpleControls", "ngRoute", "ngSanitize"])
    .config(function ($routeProvider) {
        $routeProvider.when("/", {
            controller: "beersController",
            controllerAs: "vm",
            templateUrl: "/views/beersView.html"
        });

        $routeProvider.when("/api/beer/:beerId", {
                controller: "beerInfoController",
                controllerAs: "vm",
                templateUrl: "/views/beerInfoView.html"
        });

        $routeProvider.when("/api/brewery/:breweryId" ,{
            controller: "breweryInfoController",
            controllerAs: "vm",
            templateUrl: "/views/breweryInfoView.html"
        });

        $routeProvider.when("Beers/Brewery?BreweryId=:breweryId" ,{
            controller: "breweryBeersController",
            controllerAs: "vm",
            templateUrl: "/views/breweryBeersView.html"
        });

        $routeProvider.otherwise({redirectTo: "/"});
    })
})();