JosefinaApp.controller('TicketsOrdersController', function TicketsOrdersController($scope, $state, $http, $rootScope, ticketsOrdersViewModel, i18nService, uiGridConstants, $uibModal) {
    i18nService.setCurrentLang('cz');
    if (ticketsOrdersViewModel.data.IsValid) {
        if (ticketsOrdersViewModel.data.IsAuthorized) {
            //Orders grid
            $scope.ordersGridOptions = {
                enableHorizontalScrollbar: 0,
                enableVerticalScrollbar: 2,
                enableRowSelection: true, enableRowHeaderSelection: false,
                enableFiltering: true,
                columnDefs: [
                             { name: 'Email', field: 'Email', headerCellClass: 'header-filtered' },
                             { name: 'Objednáno', field: 'Ordered', cellFilter: 'date:\'dd.MM.yyyy\'', headerCellClass: 'header-filtered' },
                             { name: 'Zaplaceno', field: 'PaidDate', cellFilter: 'date:\'dd.MM.yyyy\'', headerCellClass: 'header-filtered' },
                             { name: 'Stav', field: 'State', headerCellClass: 'header-filtered' },
                             { name: 'Names', field: 'Names', headerCellClass: 'header-filtered' }
                ],
            };
            $scope.ordersGridOptions.multiSelect = false;
            $scope.ordersGridOptions.data = ticketsOrdersViewModel.data.Orders;
            

            $rootScope.PageTitle = ticketsOrdersViewModel.data.Title;
            $scope.ticketOrdersActive = true;
        }
        else {
            $state.go('unauthorized');
        }
    }
    else {
        $state.go('error');
    }

    $scope.ordersGridOptions.onRegisterApi = function (gridApi) {
        $scope.ordersGridApi = gridApi;
    };


    $scope.UpdateOrder = function () {
        var selectedRow = $scope.ordersGridApi.selection.getSelectedRows();

        if (selectedRow.length === 0) {
            $scope.errorModal = { Header: "Objednávka", Message: "Není vybrána objednávka k zobrazení." }

            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: 'http://localhost:44301/AngularViews/Modals/HeaderMessageDanger.html',
                controller: ModalShowError,
                scope: $scope
            });
        }
        else {
            $state.go('tickets.order', { orderID: selectedRow[0].TicketOrderID });
        }
    };       
});

var ModalShowError = function ($scope, $uibModalInstance) {
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
};