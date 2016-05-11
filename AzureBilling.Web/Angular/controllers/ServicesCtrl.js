var app = angular.module('app');
app.controller('servicesCtrl', ['$scope', '$http', 'chartService', function ($scope, $http, chartService) {

    $scope._chartService = chartService;

    var getAllCategories = function () {
        return {
            'Data Management': 'Data Management', 'Data Services': 'Data Services',
           /* 'Networking': 'Networking',*/
            'SQL Database': 'SQL Database', 'Storage': 'Storage', 'Visual Studio': 'Visual Studio'
        };
    }

     //initialize the graph
    $scope.initGraph = function () {

        //make server call to get data
        $http({
            method: 'GET',
            url: '/data/SpendingByService'
        }).then(function successCallback(response) {
            var data1 = {};
            data1.title = 'Usage by Category';
            data1.data = response.data;
            $scope._chartService.drawPieChart('container1', data1);
        });
    };

    $scope.initGraph();
}]);