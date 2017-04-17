JosefinaApp.controller('TicketsBankSettingsController', function TicketsBankSettingsController($scope, $state, $http, $rootScope, ticketsBankSettingsViewModel, $uibModal) {
    if (ticketsBankSettingsViewModel.data.IsValid) {
        if (ticketsBankSettingsViewModel.data.IsAuthorized) {
            initBank();
            $scope.showBankRegistration = true;
            $scope.ticketBankSettingsActive = true;

            $scope.showTransactionLoadBtn = true;
            $scope.showTransactionLoadMsg = false;

            initUpdateTransactions();

        }
        else {
            $state.go('unauthorized');
        }
    }
    else {
        $state.go('error');
    }

    function initBank() {
        $scope.settingsBankModel = {};
        $scope.settingsBankModel.AccountNumber = ticketsBankSettingsViewModel.data.AccountNumber;
        $scope.settingsBankModel.BankCode = 2010;
        $scope.settingsBankModel.ProjectURL = ticketsBankSettingsViewModel.data.ProjectURL;
        $scope.settingsBankModel.Token = ticketsBankSettingsViewModel.data.Token;
        $scope.settingsBankModel.BIC = ticketsBankSettingsViewModel.data.BIC;
        $scope.settingsBankModel.IBAN = ticketsBankSettingsViewModel.data.IBAN;

    };

    function initUpdateTransactions() {
        $scope.updateTransactions = {};
        $scope.updateTransactions.Selected = "false";
    };

    $scope.SubmitBankForm = function () {
        angular.forEach($scope.settingsBankProxyForm.$error.required, function (field) {
            field.$setDirty();
        });

        if ($scope.settingsBankProxyForm.$valid) {

            $scope.showBankRegistration = false;
            $scope.showBankRegistrationInProgress = true;
            bankProxyModel = {};
            bankProxyModel.ProjectID = $rootScope.projectID;
            bankProxyModel.Token = $scope.settingsBankModel.Token;

            if (ticketsBankSettingsViewModel.data.Token != undefined && ticketsBankSettingsViewModel.data.Token != '') {
                bankProxyModel.IsNew = false;
            }
            else {
                bankProxyModel.IsNew = true;
            }

            $http.post('/api/project/tickets/CreateUpdateBankProxy', bankProxyModel)
            .success(function (data) {
                $scope.showBankRegistration = true;
                $scope.showBankRegistrationInProgress = false;
                if (data.IsValid) {
                    if (data.IsAuthorized) {
                        if (!data.Error) {
                            $scope.settingsBankModel.ProjectURL = data.TicketsURL;
                            $scope.settingsBankModel.AccountNumber = data.FioBankProxyViewModel.AccountNumber;
                            $scope.settingsBankModel.BIC = data.FioBankProxyViewModel.BIC;
                            $scope.settingsBankModel.IBAN = data.FioBankProxyViewModel.IBAN;
                            ShowSuccessModal(true);
                        }
                        else {
                            $scope.errorModal = { Header: "Registrace bankovního spojení", Message: "Během zpracování požadavku se vyskytla neočekávaná chyba. Opakujte prosím danou akci později." };


                            var modalInstance = $uibModal.open({
                                animation: true,
                                templateUrl: 'http://localhost:44301/AngularViews/Modals/HeaderMessageDanger.html',
                                controller: ModalShowError,
                                scope: $scope
                            });
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

    $scope.UpdateTransactions = function () {

        angular.forEach($scope.updateTransactionsForm.$error.required, function (field) {
            field.$setDirty();
        });

        if ($scope.updateTransactionsForm.$valid) {

            $scope.showTransactionLoadBtn = false;
            $scope.showTransactionLoadMsg = true;

            var postModel = {};
            postModel.ProjectID = $rootScope.projectID;
            postModel.FromSelected = $scope.updateTransactions.Selected;
            postModel.FromDate = new Date($scope.updateTransactions.loadFrom);

            $http.post('/api/project/tickets/UpdateTransactions', postModel)
            .success(function (data) {

                $scope.showTransactionLoadBtn = true;
                $scope.showTransactionLoadMsg = false;

                if (data.IsValid) {
                    if (data.IsAuthorized) {

                        if (!data.Error) {
                            ShowSuccessModal(false);
                        }
                        else {
                            $scope.errorModal = { Header: "Registrace bankovního spojení", Message: data.ErrorMessage };


                            var modalInstance = $uibModal.open({
                                animation: true,
                                templateUrl: 'http://localhost:44301/AngularViews/Modals/HeaderMessageDanger.html',
                                controller: ModalShowError,
                                scope: $scope
                            });
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

    $scope.loadFromClick = function () {
        $scope.updateTransactions.loadFromOpen = !$scope.updateTransactions.loadFromOpen;
    };

    function ShowSuccessModal(isBank) {
        $scope.successModal = {};
        if (isBank) {
            $scope.successModal.Header = "Registrace bankovního spojení";
            $scope.successModal.Message = "Token byl úspěšně oveřen a bankovní spojení bylo registrováno.";
        }
        else
        {
            $scope.successModal.Header = "Aktualizace plateb";
            $scope.successModal.Message = "Platby byly úspěšně aktulizovány.";
        }
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'http://localhost:44301/AngularViews/Modals/Success.html',
            controller: ModalSuccessCtrl,
            scope: $scope
        });
    };

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