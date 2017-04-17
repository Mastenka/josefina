JosefinaApp.controller('TaskLowerController', function TaskLowerController($scope, $state, $http, taskViewModel, $q, $location, $rootScope) {
    if (taskViewModel.data.IsValid) {
        if (taskViewModel.data.IsAuthorized) {
            $scope.lowerViewModel = taskViewModel.data.TaskLowerViewModel;
            $scope.taskID = $location.$$path.split('/')[3];
        }
        else {
            $state.go('tasks.unauthorized');
        }
    }
    else {
        $state.go('tasks.error');
    }

    $scope.lowerViewModel.COMMENTS_PAGE_COUNT = 100;
    $scope.lowerViewModel.loadedPagesCount = 1;
    $scope.lowerViewModel.isEndOfComments = $scope.lowerViewModel.Comments.length < $scope.lowerViewModel.COMMENTS_PAGE_COUNT;

    var initialOffset = -20; // it is needed to initiate proper items fetching
    $scope.commentsDataSource = {
        get: function (index, count, callback) {
            var arr = [];
            var start = index + count + initialOffset;
            var end = Math.min(start + count - 1, 0);

            if (start <= end) {
                start *= -1;
                end *= -1;

                if (start >= $scope.lowerViewModel.Comments.length) {
                    if ($scope.lowerViewModel.isEndOfComments) {
                        start = $scope.lowerViewModel.Comments.length - 1;
                    }
                    else {
                        $scope.getCommentsPage(start, end, callback);
                        return;
                    }
                }

                for (var i = start; i >= end; i--) {
                    arr.push($scope.lowerViewModel.Comments[i]);
                }
            }
            callback(arr);
        }
    };

    $scope.getCommentsPage = function (start, end, callback) {
        var pagePromise = $scope.getCommentsPromise();

        // wait until the promise return resolve or eject
        pagePromise.then(function (resolve) {

            if (resolve.IsValid) {
                $scope.lowerViewModel.Comments = $scope.lowerViewModel.Comments.concat(resolve.Comments);

                var arr = [];

                if (start >= $scope.lowerViewModel.Comments.length) {
                    start = $scope.lowerViewModel.Comments.length - 1;
                    $scope.lowerViewModel.isEndOfComments = true;
                } else if (resolve.Comments.length < $scope.lowerViewModel.COMMENTS_PAGE_COUNT) {
                    $scope.lowerViewModel.isEndOfComments = true;
                }

                for (var i = start; i >= end; i--) {
                    arr.push($scope.lowerViewModel.Comments[i]);
                }
                callback(arr);
            }
            else {
                $state.go('tasks.error');
            }
        }, function (reject) {
            $state.go('tasks.error');
        });
    }

    $scope.getCommentsPromise = function () {

        var deferred = $q.defer();
        $scope.lowerViewModel.loadedPagesCount++;

        $http({
            method: 'GET',
            url: '/api/project/tasks/gettaskcommentspage/' + $scope.taskID + '/' + $scope.lowerViewModel.loadedPagesCount + '/' + $scope.lowerViewModel.TimeOfLoad
        })
            //if request is successful
            .success(function (data, status, headers, config) {

                //resolve the promise
                deferred.resolve(data);

            })
            //if request is not successful
            .error(function (data, status, headers, config) {
                //reject the promise
                deferred.reject('ERROR');
            });

        //return the promise
        return deferred.promise;
    };

    $scope.clicked = function () {
        if ($scope.lowerViewModel.comment === undefined || $scope.lowerViewModel.comment == "") {
            return;
        }
        var data = { "TaskID": $scope.taskID, "Content": $scope.lowerViewModel.comment, "TimeOfLoad": $scope.lowerViewModel.TimeOfLoad };

        $http.post(
                   '/api/project/tasks/postcomment',
                   JSON.stringify(data));

        $scope.lowerViewModel.comment = "";
    };

    if ($rootScope.SignalRHubConnected) {
        JoinGroup();
    }
    else {
        $scope.lowerViewModel.SignalRListener = $rootScope.$on('tasks.SignalR.Connected', function (event) {
            JoinGroup();
        });
    }

    //Subscribes to new comments updates
    function JoinGroup() {
        var data = { GroupName: 'TaskComments:' + $scope.taskID };
        $rootScope.$emit('tasks.SignalR.JoinGroup', data);

        //Listen to NewComment events through rootscope from TreeViewControllers SignalR hub
        $scope.lowerViewModel.NewCommentListener = $rootScope.$on('tasks.SignalR.NewComment', function (event, data) {
            //debugger;
            if (data.GroupID === 'TaskComments:' + $scope.taskID) {
                $scope.lowerViewModel.Comments.unshift(data.Comment);
                $scope.isReload = true;
                $scope.commentsAdapter.reload();
            }
        });

        if ($scope.lowerViewModel.SignalRListener !== undefined) {
            $scope.lowerViewModel.SignalRListener();
        }
    };

    $scope.$on('$destroy', function () {
        var data = { GroupName: 'TaskComments:' + $scope.taskID };
        $rootScope.$emit('tasks.SignalR.LeaveGroup', data);
        $scope.commentsAdapter = null;
        $scope.commentsDataSource = null;
        $scope.lowerViewModel.NewCommentListener();
    });




    $scope.$on('elastic:resize', function (event, element, oldHeight, newHeight) {
        var isNewLine = element.context.value.indexOf('\n') > -1;

        if (element.context.innerText !== undefined && !isNewLine) {
            isNewLine = element.context.innerHTML.substr(element.context.innerText.length) === "\n";
        }

        if (isNewLine) {
            var data = { "TaskID": $scope.taskID, "Content": $scope.lowerViewModel.comment, "TimeOfLoad": $scope.lowerViewModel.TimeOfLoad };
            $http.post(
                       '/api/project/tasks/postcomment',
                       JSON.stringify(data));
            $scope.lowerViewModel.comment = "";
            element.context.innerHTML = "";

        }
    });

    //$scope.$on('ui.layout.toggle', function (e, container) {
    //    //debugger;
    //    if (container.size > 0) {
    //        console.log('container is open!');
    //    }
    //});

    //$scope.$on('ui.layout.resize', function (e, beforeContainer, afterContainer) { });

    //Ctr end
});
