var app = angular.module('app');
app.controller('accountsCtrl', ['$scope', '$http','chartService', function ($scope, $http,chartService) {

    $scope._chartService = chartService;

    $scope.initGraph = function () {

        //make server call to get data
        $http({
            method: 'GET',
            url: '/data/SpendingByAccount'
        }).then(function successCallback(response) {
            var data = {};
            data.title = 'Cost By Account Name',
            data.data = response.data
            data.series = response.data.data;
            chartService.drawPieChart('container1', data);
        });
    };

    $scope.initGraph();
}]);