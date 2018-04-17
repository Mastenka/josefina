JosefinaApp.controller('TaskPropertiesController', function TaskPropertiesController($scope, $state, $http, taskViewModel, $uibModal, $rootScope) {
    if (taskViewModel.data.IsValid) {
        if (taskViewModel.data.IsAuthorized) {
            //debugger;
            $scope.propertiesViewModel = taskViewModel.data.TaskPropertiesViewModel;

            if ($scope.propertiesViewModel.Subscribed) {
                $scope.propertiesViewModel.btnSubscribeLabel = 'Nesledovat změny';
            }
            else {
                $scope.propertiesViewModel.btnSubscribeLabel = 'Sledovat změny';
            }

            if ($scope.propertiesViewModel.Completed) {
                $scope.propertiesViewModel.btnCompleteTaskLabel = "Otevřít úkol";
            }
            else {
                $scope.propertiesViewModel.btnCompleteTaskLabel = "Uzavřít úkol";
            }
        }
        else {
            $state.go('tasks.unauthorized');  
        }
    }
    else {
        $state.go('tasks.error');
    }

    $scope.btnEditContent = function () {
        $http({
            method: 'GET',
            url: '/api/project/tasks/getedittaskcontent/' + $state.params.taskId,
        }).
        success(function (data) {
            if (!data.IsLocked) {
                $rootScope.Task = { ContentToEdit: data.Content };
                $state.go('tasks.task.detail.edit', { taskId: $state.params.taskId }, { location: false });
            }
            else {
                $scope.errorModal = { Header: "Upravit obsah", Message: "Obsah úkolu je momentálně upravován uživatelem: " + data.UserName + "." }

                $scope.cancel = function () {
                    debugger;
                    $uibModalInstance.dismiss('cancel');
                };

                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'http://localhost:44301/AngularViews/Modals/HeaderMessageDanger.html',
                    controller: ModalShowError,
                    scope: $scope
                });
            }
        }).
        error(function () {
            $scope.test = "Error";
        });
    };

    $scope.btnWatchContent = function () {
        if ($scope.propertiesViewModel.Subscribed) {//Unsubscribe
            $scope.propertiesViewModel.Subscribed = false;
            $http.get(
                    '/api/project/tasks/unsubscribetoupdates/' + $state.params.taskId);
            $scope.propertiesViewModel.btnSubscribeLabel = 'Sledovat změny';
        }
        else { //Subscribe
            $scope.propertiesViewModel.Subscribed = true;
            $http.get(
                    '/api/project/tasks/subscribetoupdates/' + $state.params.taskId);
            $scope.propertiesViewModel.btnSubscribeLabel = 'Nesledovat změny';
        }
    };

    $scope.btnCompleteTask = function () {
        if ($scope.propertiesViewModel.TaskCompleted) {//Reopen
            $scope.propertiesViewModel.TaskCompleted = false;
            $scope.propertiesViewModel.btnCompleteTaskLabel = "Uzavřít úkol";
        }
        else { //Close
            $scope.propertiesViewModel.TaskCompleted = true;
            $scope.propertiesViewModel.btnCompleteTaskLabel = "Otevřít úkol";

        }
        $http.post('/api/project/tasks/changetaskstate', { Completed: $scope.propertiesViewModel.TaskCompleted, TaskId: $state.params.taskId });
    };
});

var ModalShowError = function ($scope, $uibModalInstance) {
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
};