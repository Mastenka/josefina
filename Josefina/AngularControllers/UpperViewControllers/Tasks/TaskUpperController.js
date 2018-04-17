JosefinaApp.controller('TaskUpperController', function TaskUpperController($scope, $state, $http, taskViewModel, $rootScope, $location) {
    if (taskViewModel.data.IsValid) {
        if (taskViewModel.data.IsAuthorized) {
            $scope.upperViewModel = taskViewModel.data.TaskUpperViewModel;
            $scope.Content = taskViewModel.data.TaskUpperViewModel.Content;
            $rootScope.PageTitle = taskViewModel.data.Title;
            $scope.taskID = $location.$$path.split('/')[3];
        }
        else {
            $state.go('tasks.unauthorized');
        }
    }
    else {
        $state.go('tasks.error');
    }

    if ($rootScope.SignalRHubConnected) {
        JoinGroup();
    }
    else {
        $scope.upperViewModel.SignalRListener = $rootScope.$on('tasks.SignalR.Connected', function (event) {
            JoinGroup();
        });
    }

    //Subscribes to new comments updates
    function JoinGroup() {
        var data = { GroupName: 'TaskContent:' + $scope.taskID };
        $rootScope.$emit('tasks.SignalR.JoinGroup', data);

        //Listen to NewComment events through rootscope from TreeViewControllers SignalR hub
        $scope.upperViewModel.NewCommentListener = $rootScope.$on('tasks.SignalR.UpdateContent', function (event, data) {
            //debugger;
            if (data.GroupID === 'TaskContent:' + $scope.taskID) {
                $scope.$apply(function () {
                    $scope.Content = data.Content;
                });
            }
        });

        if ($scope.upperViewModel.SignalRListener !== undefined) {
            $scope.upperViewModel.SignalRListener();
        }
    };

    $scope.$on('$destroy', function () {
        var data = { GroupName: 'TaskContent:' + $scope.taskID };
        $rootScope.$emit('tasks.SignalR.LeaveGroup', data);
        $scope.upperViewModel.NewCommentListener();
    });
});