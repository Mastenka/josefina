JosefinaApp.controller('TaskEditUpperController', function TaskEditUpperController($scope, $state, $http, taskViewModel, $rootScope, $interval) {
    $scope.ContentToEdit = $rootScope.Task.ContentToEdit;
    $scope.editSaved = false;

    var eventListenerToDestroy = $rootScope.$on('tasks.taks.edit.save', function (event) {
        if ($scope.editSaved) {
            return;
        }
        $scope.editSaved = true;
        var data = { 'TaskID': $state.params.taskId, 'Content': $scope.ContentToEdit };
        $http.post('/api/project/tasks/postcontentedit',
                    JSON.stringify(data));
        $state.go('tasks.task.detail', { taskId: $state.params.taskId, triggerToUpdate: "ToUpdate" }, { location: true });
    });

    var heartbeatInterval = $interval(function () {
        $http({
            method: 'GET',
            url: '/api/project/tasks/GetTaskEditHeartbeatResponse/' + $state.params.taskId,
        });
    }, 60000); //~1min


    $scope.$on('$destroy', function () {
        $interval.cancel(heartbeatInterval);
        eventListenerToDestroy();
    });
    //Ctr end
});