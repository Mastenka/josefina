JosefinaApp.controller('TasksTreeViewController', function TasksTreeViewController($scope, $location, $http, $uibModal, $state, $stateParams, $rootScope) {
    $scope.folderModal = { header: "", name: "", button: "" };
    $scope.ProjectId = $location.absUrl().split('#')[0].split('/').pop();
    $http({
        method: 'GET',
        url: '/api/project/tasks/gettasksviewmodel/' + $scope.ProjectId,
    }).
    success(function (viewModel) {
        if (viewModel.IsValid) {
            if (viewModel.IsAuthorized) {
                $scope.treeModel = viewModel.TreeViewNodes;
                $scope.treeModelLocal = angular.copy(viewModel.TreeViewNodes);
                if ($rootScope.PageTitle === undefined) {
                    $rootScope.PageTitle = viewModel.Title;
                }
                $scope.projectTitle = viewModel.Title;
            }
            else {
                $state.go('unauthorized');
            }
        } 
        else {
            $state.go('error');
        }
    }). 
    error(function () {
        $scope.test = "Error";
    });

    $scope.treeViewNodeOpened = function (e, data) {
        var id = data.node.id;
        for (var i = 0; i < $scope.treeModelLocal.length; i++) {
            if ($scope.treeModelLocal[i].id === id) {
                $scope.treeModelLocal[i].state.opened = true;
                break;
            }
        }
    };

    $scope.treeViewNodeClosed = function (e, data) {
        var id = data.node.id;
        for (var i = 0; i < $scope.treeModelLocal.length; i++) {
            if ($scope.treeModelLocal[i].id === id) {
                $scope.treeModelLocal[i].state.opened = false;
                break;
            }
        }
    };

    $scope.treeViewNodeSelected = function (e, data) {

        $scope.selectedNode = data.node;

        if (data.event.button !== 0) {
            $scope.rightSelectedNodeId = data.node.id;
            //console.log('Rigth-Click: ' + $scope.rightSelectedNodeId);
            return;
        }
        else {
            //console.log('Left-Click: ' + $scope.rightSelectedNodeId);
            $scope.selectedNodeId = data.node.id;
            $scope.rightSelectedNodeId = data.node.id;
            if ($scope.selectedNodeId[0] === 'T') {
                for (var i = 0; i < $scope.treeModelLocal.length; i++) {
                    if ($scope.treeModelLocal[i].id === $scope.selectedNodeId) {

                        var segments = $scope.treeModelLocal[i].text.split('<');
                        if (segments.length > 1) {

                            $scope.treeModelLocal[i].text = segments[0];
                            $scope.treeModel = angular.copy($scope.treeModelLocal);
                            $scope.treeModel[i].state.selected = true;
                            break;
                        }
                    }
                }

                var taskID = $scope.selectedNodeId.substr(1);
                $state.go('tasks.task.detail', { taskId: taskID });
            }
            else if ($scope.selectedNodeId[0] === 'F') {
                //$rootScope.PageTitle = $scope.projectTitle;
                //$state.go('tasks.task');
            }
            else if ($scope.selectedNodeId[0] === 'P') {
                //$rootScope.PageTitle = $scope.projectTitle;
                $state.go('tasks');
            }
        }
    };

    $scope.contextMenu = {
        "Menu 1": {
            "label": "Vytvořit složku",
            "id": "1",
            "action": function (obj) {
                if ($scope.rightSelectedNodeId === undefined || $scope.rightSelectedNodeId === "") {
                    return;
                }
                //console.log("New folder: " + $scope.rightSelectedNodeId);

                if ($scope.rightSelectedNodeId[0] === 'F' || $scope.rightSelectedNodeId[0] === 'P') {
                    $scope.folderModal = { header: "Nová složka", name: "", button: "Vytvořit" };
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'http://localhost:44301/AngularViews/Modals/UpdateCreateNode.html',
                        controller: ModalCreateFolderCtrl,
                        scope: $scope
                    });
                }
            }
        },
        "Menu 2": {
            "label": "Vytvořit úkol",
            "id": "2",
            "action": function (obj) {
                if ($scope.rightSelectedNodeId === undefined || $scope.rightSelectedNodeId === "") {
                    return;
                }
                //console.log("New task: " + $scope.rightSelectedNodeId);

                if ($scope.rightSelectedNodeId[0] === 'F' || $scope.rightSelectedNodeId[0] === 'P') {
                    $scope.taskModal = { header: "Nový úkol", name: "", button: "Vytvořit" };
                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'http://localhost:44301/AngularViews/Modals/UpdateCreateTask.html',
                        controller: ModalCreateTaskCtrl,
                        scope: $scope
                    });
                }
            }
        },   
        "Menu 3": {
            "label": "Smazat",
            "id": "3",
            "action": function (obj) {
                if ($scope.rightSelectedNodeId === undefined || $scope.rightSelectedNodeId === "") {
                    return;
                }
                if ($scope.rightSelectedNodeId[0] === 'P') {
                    return;
                }
                $scope.confirmationModal = {};
                $scope.confirmationModal.button1 = "Ano";
                $scope.confirmationModal.button2 = "Ne";

                //console.log("Delete: " + $scope.rightSelectedNodeId);
                if ($scope.rightSelectedNodeId[0] === 'F') {
                    $scope.confirmationModal.Header = "Smazání složky";
                    $scope.confirmationModal.Message = "Opravdu smazat danou složku?";
                }

                if ($scope.rightSelectedNodeId[0] === 'T') {
                    $scope.confirmationModal.Header = "Smazání úkolu";
                    $scope.confirmationModal.Message = "Opravdu smazat daný úkol?";
                }
                $scope.confirmationModal.NodeId = $scope.rightSelectedNodeId;
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'http://localhost:44301/AngularViews/Modals/Confirmation.html',
                    controller: ModalDeleteCtrl,
                    scope: $scope
                });
            }
        },
        "Menu 4": {
            "label": "Editovat",
            "id": "4",
            "action": function (obj) {
                if ($scope.rightSelectedNodeId === undefined || $scope.rightSelectedNodeId === "") {
                    return;
                }
                if ($scope.rightSelectedNodeId[0] === 'T') {

                    var selectedNode = GetRightSelectedNod();

                    $scope.taskModal = {
                        header: "Editace", name: selectedNode.text,
                        button: "Uložit",
                        deadLine: selectedNode.deadline, taskID: $scope.rightSelectedNodeId.replace('T', '')
                    };

                    $scope.taskModal.enableDeadline = $scope.taskModal.deadLine !== "";

                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'http://localhost:44301/AngularViews/Modals/UpdateCreateTask.html',
                        controller: ModalUpdateTaskCtrl,
                        scope: $scope
                    });
                }
                else if ($scope.rightSelectedNodeId[0] === 'F') {
                      $scope.folderModal = {};
                var oldName = "";
                for (var i = 0; i < $scope.treeModelLocal.length; i++) {
                    if ($scope.treeModelLocal[i].id == $scope.rightSelectedNodeId) {
                        oldName = $scope.treeModelLocal[i].text;
                        break;
                    }
                }
                $scope.folderModal = { header: "Editace", name: oldName, button: "Uložit" };
                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'http://localhost:44301/AngularViews/Modals/UpdateCreateNode.html',
                    controller: ModalRenameNodeCtrl,
                    scope: $scope
                });
                }
            }
        }
    };

    function GetRightSelectedNod() {
        for (var i = 0; i < $scope.treeModelLocal.length; i++) {
            if ($scope.treeModelLocal[i].id === $scope.rightSelectedNodeId) {
                return $scope.treeModelLocal[i];
            }
        }
    };

    //Main SignalR entry point
    $(function () {
        //$.connection.hub.logging = true;
        //Update TreeView model/Nodes
        $.connection.tasksHub.client.updateTreeView = function updateTreeView(groupID, node, isNew) {
            console.log("TV UPdate: " + groupID);
            $scope.$apply(function () {
                if (groupID === 'TasksTreeNodes:' + $scope.ProjectId) {
                    if (isNew) {
                        $scope.treeModelLocal.push(node);
                        $scope.treeModel = angular.copy($scope.treeModelLocal);
                    }
                    else {
                        for (var i = 0; i < $scope.treeModelLocal.length; i++) {
                            if ($scope.treeModelLocal[i].id === node.id) {
                                node.state = $scope.treeModelLocal[i].state;
                                $scope.treeModelLocal[i] = node;
                                $scope.treeModel = angular.copy($scope.treeModelLocal);
                                break;
                            }
                        }
                    }
                }
            });
        };

        //New comment for Comments View
        $.connection.tasksHub.client.newComment = function newComment(comment) {
            $rootScope.$emit('tasks.SignalR.NewComment', comment);
        };

        //Task content updated 
        $.connection.tasksHub.client.updateContent = function updateContent(content) {
            $rootScope.$emit('tasks.SignalR.UpdateContent', content);
        };

        //Task properties updated 
        $.connection.tasksHub.client.updateProperties = function updateProperties(properties) {
            $rootScope.$emit('tasks.SignalR.UpdateProperties', properties);
        };

        $.connection.hub.start().done(function () {
            var groupName = 'TasksTreeNodes:' + $scope.ProjectId;
            $.connection.tasksHub.server.joinGroup(groupName);
            $rootScope.SignalRHubConnected = true;
            $rootScope.$emit('tasks.SignalR.Connected');
        });
    });

    $rootScope.$on('tasks.SignalR.JoinGroup', function (event, data) {
        $.connection.tasksHub.server.joinGroup(data.GroupName);
    });

    $rootScope.$on('tasks.SignalR.LeaveGroup', function (event, data) {
        $.connection.tasksHub.server.leaveGroup(data.GroupName);
    });

    $scope.$on('$destroy', function () {
        var groupName = 'TasksTreeNodes:' + $scope.ProjectId;
        $.connection.tasksHub.server.leaveGroup(groupName);
        $.connection.hub.stop();
    });

    $scope.$on('createFolder', function (event, name) {
        //console.log("Create folder event: " + $scope.rightSelectedNodeId);
        $http.post('/api/project/tasks/CreateFolder', { NodeId: $scope.rightSelectedNodeId, Name: name, ProjectId: $scope.ProjectId });
    });

    $scope.$on('createTask', function (event, name, deadLine, withDeadline) {
        //console.log("Create task event: " + $scope.rightSelectedNodeId);
        $http.post('/api/project/tasks/CreateTask', { NodeId: $scope.rightSelectedNodeId, Name: name, ProjectId: $location.absUrl().split('#')[0].split('/').pop(), DeadLine: deadLine, WithDeadline: withDeadline });
    });

    $scope.$on('deleteNode', function (event, nodeId) {
        //console.log("Delete event: " + $scope.rightSelectedNodeId);
        $http.get('/api/project/tasks/deletenode/' + nodeId)
            .success(function (data, status, headers, config) {
                if (data.Success) {
                    for (var i = 0; i < $scope.treeModelLocal.length; i++) {
                        if ($scope.treeModelLocal[i].id == nodeId) {
                            $scope.treeModelLocal.splice(i, 1);
                        }
                    }
                    $scope.treeModel = angular.copy($scope.treeModelLocal);
                }
                else {
                    $scope.errorModal = { Header: "Chyba", Message: data.ErrorMessage }

                    var modalInstance = $uibModal.open({
                        animation: true,
                        templateUrl: 'http://localhost:44301/AngularViews/Modals/HeaderMessageDanger.html',
                        controller: ModalShowError,
                        scope: $scope
                    });
                }

            })
    });

    $scope.$on('renameNode', function (event, newName) {

        //console.log("Delete event: " + $scope.rightSelectedNodeId);
        $http.post('/api/project/tasks/changename', { TreeNodeID: $scope.rightSelectedNodeId, Name: newName });
    });
});

var ModalCreateFolderCtrl = function ($scope, $uibModalInstance) {
    var ignoreSubmit = false;

    $scope.submitForm = function () {
        if (ignoreSubmit) {
            return;
        }

        if ($scope.folderModal.name != undefined && $scope.folderModal.name != "") {
            $scope.$emit('createFolder', $scope.folderModal.name);
            $uibModalInstance.close('closed');
        }
    };
    $scope.cancel = function () {
        ignoreSubmit = true;
        $uibModalInstance.close('closed');
    };
};

var ModalCreateTaskCtrl = function ($scope, $uibModalInstance) {
    $scope.taskModal.deadlineOpen = false;
    $scope.taskModal.enableDeadline = false;
    var ignoreSubmit = false;

    $scope.submitForm = function () {
        if (ignoreSubmit) {
            return;
        }
        if ($scope.taskModal.name != undefined && $scope.taskModal.name != "" &&
            (!$scope.taskModal.enableDeadline ||
            ($scope.taskModal.deadLine != "" && $scope.taskModal.deadLine != undefined))) {
            $scope.$emit('createTask', $scope.taskModal.name, $scope.taskModal.deadLine, $scope.taskModal.enableDeadline);
            $uibModalInstance.close('closed');
        }
    };
    $scope.deadlineClick = function () {
        $scope.taskModal.deadlineOpen = !$scope.taskModal.deadlineOpen;
    };
    $scope.cancel = function () {
        ignoreSubmit = true;
        $uibModalInstance.close('closed');
    };
};

var ModalUpdateTaskCtrl = function ($scope, $uibModalInstance, $http) {
    $scope.taskModal.deadlineOpen = false;
    var ignoreSubmit = false;

    $scope.submitForm = function () {
        if (ignoreSubmit) {
            return;
        }
        if ($scope.taskModal.name != undefined && $scope.taskModal.name != "" &&
            (!$scope.taskModal.enableDeadline ||
            ($scope.taskModal.deadLine != "" && $scope.taskModal.deadLine != undefined))) {
            
            $http.post('/api/project/tasks/changetaskstate', {
                TaskID: $scope.taskModal.taskID, Completable: $scope.taskModal.enableDeadline,
                NewDeadline: $scope.taskModal.deadLine, Name: $scope.taskModal.name
            });

            $uibModalInstance.close('closed');
        }
    };
    $scope.deadlineClick = function () {
        $scope.taskModal.deadlineOpen = !$scope.taskModal.deadlineOpen;
    };
    $scope.cancel = function () {
        ignoreSubmit = true;
        $uibModalInstance.close('closed');
    };
};

var ModalDeleteCtrl = function ($scope, $uibModalInstance) {

    var ignoreSubmit = false;

    $scope.click1 = function () {
        if (ignoreSubmit) {
            return;
        }
        $scope.$emit('deleteNode', $scope.confirmationModal.NodeId);
        $uibModalInstance.close('closed');
    };

    $scope.click2 = function () {
        ignoreSubmit = true;
        $uibModalInstance.close('closed');
    };

    $scope.cancel = function () {
        ignoreSubmit = true;
        $uibModalInstance.close('closed');
    };
};

var ModalRenameNodeCtrl = function ($scope, $uibModalInstance) {
    var ignoreSubmit = false;

    $scope.submitForm = function () {
        if (ignoreSubmit) {
            return;
        }
        if ($scope.folderModal.name != undefined && $scope.folderModal.name != "") {
            $scope.$emit('renameNode', $scope.folderModal.name);
            $uibModalInstance.close('closed');
        }
    };
    $scope.cancel = function () {
        ignoreSubmit = true;
        $uibModalInstance.close('closed');
    };
};

var ModalShowError = function ($scope, $uibModalInstance) {
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
};
