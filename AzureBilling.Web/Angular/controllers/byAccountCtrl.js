var app = angular.module('app');
app.controller('byAccountCtrl', ['$scope', '$http','chartService', function ($scope, $http,chartService) {

    $scope._chartService = chartService;

    $scope.onFilterSelected = function(id)
    {
        var key = 'id_'+id;
        if($scope.selectedAccounts[key])
        {
            delete $scope.selectedAccounts[key];
        }
        else
        {
            $scope.selectedAccounts[key] = id;
        }

        $scope.$broadcast('filterChanged');
    }

    var getSelectedAccount = function () { return [] };
    
    $scope.$on('filterChanged', function () {
        $scope.initGraph();
    });
    $scope.accounts = [{ id: '1', text: 'Vishwas-Account' }, { id: '2', text: 'Jacks-Account' }];
    $scope.dates = [{ id: '1', text: '2016-Feb' }, { id: '2', text: '2016-Mar' }];
    $scope.selectedAccounts = [];

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

        //$http({
        //    method: 'GET',
        //    url: '/data/SpendingBySubscription'
        //}).then(function successCallback(response) {
        //    var data = {};
        //    data.title = 'Consumption by Department',
        //    data.data = response.data
        //    data.series = response.data.data;
        //    $scope._chartService.drawBarChart('container2', data);
        //});

        //$http({
        //    method: 'GET',
        //    url: '/data/Spending'
        //}).then(function successCallback(response) {
        //    var data = {};
        //    data.title = 'Daily Usage',
        //    data.categories = ['1', '2', '3', '4', '5', '6', '7', '8', '9', '10',
        //                '11', '12', '13', '14', '15', '16', '17', '18', '19', '20',
        //                '21', '22', '23', '24', '25', '26', '27', '28', '29', '30', ];
        //    data.series = [{ name: '', data: response.data.data }];
        //    $scope._chartService.drawLineChart('container3', data);
        //});
    };

    $scope.initGraph();
}]);