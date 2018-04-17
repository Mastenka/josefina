JosefinaApp.controller('TaskEditPropertiesController', function TaskEditPropertiesController($scope, $state, $http, taskViewModel, $rootScope) {
    $scope.ContentToEdit = $rootScope.Task.ContentToEdit;

    $scope.btnSaveChanges = function () {
        $rootScope.$emit('tasks.taks.edit.save');
    };
    
    $scope.btnCancelChanges = function () {
        //debugger;
        $http({
            method: 'GET',
            url: '/api/project/tasks/cancelcontentedit/' + $state.params.taskId,
        });
        $state.go('tasks.task.detail', { taskId: $state.params.taskId });
    };
});