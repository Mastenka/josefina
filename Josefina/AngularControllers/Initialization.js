JosefinaApp.run(function ($rootScope, $location) {
    $rootScope.projectID = $location.absUrl().split('#')[0].split('/').pop();
});