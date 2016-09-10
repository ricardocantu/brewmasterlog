//app-locations.js
(function(){

    "use strict";

    angular.module("app-locations", ["simpleControls", "ngRoute", "ngSanitize"])
    .config(function ($routeProvider) {
        $routeProvider.when("/", {
            controller: "locationsController",
            controllerAs: "vm",
            templateUrl: "/views/locationsView.html"
        });

        $routeProvider.when("/api/location/:locationId", {
                controller: "locationInfoController",
                controllerAs: "vm",
                templateUrl: "/views/locationInfoView.html"
        });

        $routeProvider.otherwise({redirectTo: "/"});
    })

})();