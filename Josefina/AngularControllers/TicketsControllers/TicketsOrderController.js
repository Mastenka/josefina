JosefinaApp.controller('TicketsOrderController', function TicketsOrderController($scope, $state, $http, $rootScope, orderViewModel, $uibModal) {
    if (orderViewModel.data.IsValid) {
        if (orderViewModel.data.IsAuthorized) {
            initTickets();
            initGrid();

            $scope.ticketOrdersActive = true;
        }
        else {
            $state.go('unauthorized');
        }
    }
    else {
        $state.go('error');
    }

    function initTickets() {
        $scope.orderModel = {};
        $scope.orderModel.Email = orderViewModel.data.Email;
        $scope.orderModel.TicketOrderID = orderViewModel.data.TicketOrderID;
        $scope.orderModel.PaidDate = orderViewModel.data.PaidDate;
        $scope.orderModel.Created = orderViewModel.data.Created;
        $scope.orderModel.Canceled = orderViewModel.data.Canceled;
        $scope.orderModel.Paid = orderViewModel.data.Paid;
        $scope.orderModel.TotalPaid = orderViewModel.data.TotalPaid;
        $scope.orderModel.TotalPrice = orderViewModel.data.TotalPrice;
        $scope.orderModel.VariableSymbol = orderViewModel.data.VariableSymbol;
        $scope.orderModel.TermsConditionsAccepted = orderViewModel.data.TermsConditionsAccepted;
    };

    function initGrid() {
        $scope.ticketItemsGridOptions = {
            enableHorizontalScrollbar: 0,
            enableVerticalScrollbar: 2,
            enableRowSelection: true, enableRowHeaderSelection: false,
            columnDefs: [
                         { name: 'Kategorie', field: 'Category', enableCellEdit: false, width: '15%' },
                         { name: 'ID/Kód', field: 'Code', enableCellEdit: false, width: '15%' },
                         { name: 'Email', field: 'Email', enableCellEdit: true, width: '35%' },
                         { name: 'Jméno', field: 'Name', enableCellEdit: true, width: '35%' }
            ],
        };
        $scope.ticketItemsGridOptions.multiSelect = false;
        $scope.ticketItemsGridOptions.data = orderViewModel.data.TicketItems;
    };

    $scope.ticketItemsGridOptions.onRegisterApi = function (gridApi) {
        $scope.ticketItemsGridApi = gridApi;
    };

    $scope.submitOrderUpdate = function () {
        angular.forEach($scope.orderForm.$error.required, function (field) {
            field.$setDirty();
        });

        if ($scope.orderForm.$valid) {

            var orderSubmitModel = $scope.orderModel;
            orderSubmitModel.TicketItems = $scope.ticketItemsGridOptions.data;

            $http.post('/api/project/tickets/UpdateTicketOrder', orderSubmitModel)
            .success(function (data) {
                if (data.IsValid) {
                    if (data.IsAuthorized) {
                        ShowSuccessModal('update');
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

    $scope.resendTickets = function () {
        

        $http.get('/api/project/tickets/ResendTickets/' + $scope.orderModel.TicketOrderID)
            .success(function (data) {
                if (data.IsValid) {
                    if (data.IsAuthorized) {
                        ShowSuccessModal('resend');
                    }
                    else {
                        $state.go('unauthorized');
                    }
                }
                else {
                    $state.go('error');
                }
            });

    };

    $scope.recreateUsers = function () {


        $http.get('/api/project/tickets/RecreateUsersJT/' + $scope.orderModel.TicketOrderID)
            .success(function (data) {
                if (data.IsValid) {
                    if (data.IsAuthorized) {
                        ShowSuccessModal('recreate');
                    }
                    else {
                        $state.go('unauthorized');
                    }
                }
                else {
                    $state.go('error');
                }
            });

    };

    function ShowSuccessModal(section) {
        $scope.successModal = {};
        switch (section) {
            case 'update':
                $scope.successModal.Header = "Uložení změn";
                $scope.successModal.Message = "Změny byly úspěšně uloženy.";
                break;
            case 'resend':
                $scope.successModal.Header = "Odeslání vstupenek";
                $scope.successModal.Message = "Vstupenky byly úspěšně odeslány.";
                break;
            case 'recreate':
                $scope.successModal.Header = "Vytvoření uživatelů";
                $scope.successModal.Message = "Uživatelé úspěšně vytvořeni.";
                break;
        }
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'http://localhost:44301/AngularViews/Modals/Success.html',
            controller: ModalSuccessCtrl,
            scope: $scope
        });
    };
});

var ModalSuccessCtrl = function ($scope, $uibModalInstance) {
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
};