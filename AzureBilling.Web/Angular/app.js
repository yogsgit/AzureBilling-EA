/// <reference path="templates/subscription-details.html" />
/// <reference path="templates/subscription-details.html" />
var app = angular.module('app', ['ngRoute','ngCookies']);
app.config(['$routeProvider',function ($routeProvider) {

    //================================================
    // Routes
    //================================================

    $routeProvider.when('/dash/:monthId?', {
        templateUrl: '/angular/templates/dashboard.html',
        controller: 'homeCtrl'
    });
    $routeProvider.when('/byAccount', {
        templateUrl: '/angular/templates/byAccount.html',
        controller: 'byAccountCtrl'
    });
    $routeProvider.when('/byService', {
        templateUrl: '/angular/templates/byServices.html',
        controller: 'byServicesCtrl'
    });
    $routeProvider.when('/signin/:message?', {
        templateUrl: 'App/SignIn',
        controller: 'signInCtrl'
    });
    $routeProvider.when('/todomanager', {
        templateUrl: 'App/TodoManager',
        controller: 'todoManagerCtrl'
    });
    $routeProvider.when('/subscriptionDetail', {
        templateUrl: '/angular/templates/subscription-details.html',
        controller: 'subscriptionCtrl'
    });

    $routeProvider.otherwise({
        redirectTo: '/dash'
    });
}]);

app.run();