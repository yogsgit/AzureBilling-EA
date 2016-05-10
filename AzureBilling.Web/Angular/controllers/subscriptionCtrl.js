var app = angular.module('app');
app.controller('subscriptionCtrl', ['$scope', '$http', function ($scope, $http) {

    $scope.subscriptions = [{
        id: "ea2939ff-05e6-4818-9a45-6cb7a2902695",
        orgId:"123", name: "Windows Azure  MSDN - Visual Studio Premium", offerId: "", currency: "", region: ""
    },

        { id: "77736167-68b0-438d-bf86-3e17e4446d94", orgId: "125", name: "Visual Studio Enterprise", offerId: "", currency: "", region: "" }];

    $scope.submitEdit = function()
    {
        for(i=0;i<$scope.subscriptions.length;i++)
        {
            console.log($scope.subscriptions[i].offerId);
            console.log($scope.subscriptions[i].orgId);
        }
    }
}]);