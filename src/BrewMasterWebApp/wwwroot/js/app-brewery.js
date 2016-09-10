//app-brewery.js

(function () {
    
    "use strict";

    angular.module("app-brewery", ["simpleControls", "ngRoute", "ngSanitize"])
    .config(function ($routeProvider) {
        $routeProvider.when("/", {
            controller: "breweryBeersController",
            controllerAs: "vm",
            templateUrl: "/views/breweryBeersView.html"
        });

        $routeProvider.otherwise({redirectTo: "/"});
    })
})();