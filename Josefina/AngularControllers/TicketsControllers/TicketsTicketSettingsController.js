JosefinaApp.controller('TicketsTicketSettingsController', function TicketsTicketSettingsController($scope, $state, $http, $rootScope, ticketsTicketSettingsViewModel, $uibModal) {
    if (ticketsTicketSettingsViewModel.data.IsValid) {
        if (ticketsTicketSettingsViewModel.data.IsAuthorized) {
            initTickets();
            $scope.ticketSettingsActive = true;
            $scope.ticketTicketSettingsActive = true;
        }
        else {
            $state.go('unauthorized');
        }
    }
    else {
        $state.go('error');
    }

    function initTickets() {
        $scope.settingsTicketsModel = {};
        $scope.settingsTicketsModel.ProjectNameCZ = ticketsTicketSettingsViewModel.data.ProjectNameCZ;
        $scope.settingsTicketsModel.ProjectNameEN = ticketsTicketSettingsViewModel.data.ProjectNameEN;
        $scope.settingsTicketsModel.LocationCZ = ticketsTicketSettingsViewModel.data.LocationCZ;
        $scope.settingsTicketsModel.LocationEN = ticketsTicketSettingsViewModel.data.LocationEN;
        $scope.settingsTicketsModel.StartsCZ = ticketsTicketSettingsViewModel.data.StartsCZ;
        $scope.settingsTicketsModel.StartsEN = ticketsTicketSettingsViewModel.data.StartsEN;
        $scope.settingsTicketsModel.MaxTicketsPerMail = ticketsTicketSettingsViewModel.data.MaxTicketsPerMail;
        $scope.settingsTicketsModel.NamedTickets = ticketsTicketSettingsViewModel.data.NamedTickets;
        $scope.settingsTicketsModel.LogoURL = ticketsTicketSettingsViewModel.data.LogoURL;
        $scope.settingsTicketsModel.NoteTicketCZ = ticketsTicketSettingsViewModel.data.NoteTicketCZ;
        $scope.settingsTicketsModel.NoteTicketEN = ticketsTicketSettingsViewModel.data.NoteTicketEN;
        $scope.settingsTicketsModel.NoteOrderCZ = ticketsTicketSettingsViewModel.data.NoteOrderCZ;
        $scope.settingsTicketsModel.NoteOrderEN = ticketsTicketSettingsViewModel.data.NoteOrderEN;
    };

    $scope.uploadPic = function (file) {
        file.upload = Upload.upload({
            url: 'https://angular-file-upload-cors-srv.appspot.com/upload',
            data: { username: $scope.username, file: file },
        });

        file.upload.then(function (response) {
            $timeout(function () {
                file.result = response.data;
            });
        }, function (response) {
            if (response.status > 0)
                $scope.errorMsg = response.status + ': ' + response.data;
        }, function (evt) {
            // Math.min is to fix IE which reports 200% sometimes
            file.progress = Math.min(100, parseInt(100.0 * evt.loaded / evt.total));
        });
    }

    $scope.submitTicketsForm = function () {

        angular.forEach($scope.settingsTicketsForm.$error.required, function (field) {
            field.$setDirty();
        });

        if ($scope.settingsTicketsForm.$valid) {
            ticketsSettingsData = $scope.settingsTicketsModel;
            ticketsSettingsData.ProjectID = $rootScope.projectID;

            $http.post('/api/project/tickets/SaveTicketsSettings', ticketsSettingsData)
            .success(function (data) {
                if (data.IsValid) {
                    if (data.IsAuthorized) {

                        if (data.Error) {
                            ShowErrorModal(data.ErrorMessage)
                        }
                        else {
                            ShowSuccessModal();
                        }
                    }
                    else {
                        $state.go('unauthorized');
                    }
                }
                else {
                    $state.go('error');
                }
            });
        }
    };

    $scope.cancelTicketsForm = function () {
        initTickets();
    };

    function ShowSuccessModal() {
        $scope.successModal = {};
        $scope.successModal.Header = "Uložení nastevní";
        $scope.successModal.Message = "Nastavení bylo úspěšně uloženo.";
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'http://localhost:44301/AngularViews/Modals/Success.html',
            controller: ModalSuccessCtrl,
            scope: $scope
        });
    };

    function ShowErrorModal(errorMessage) {
        $scope.errorModal = {};
        $scope.errorModal.Header = "Uložení nastevní";
        $scope.errorModal.Message = errorMessage;
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'http://localhost:44301/AngularViews/Modals/HeaderMessageDanger.html',
            controller: ModalShowError,
            scope: $scope
        });
    }
});

var ModalShowError = function ($scope, $uibModalInstance) {
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
};

var ModalSuccessCtrl = function ($scope, $uibModalInstance) {
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
};