JosefinaApp.controller('TicketsExportController', function TicketsExportController($scope, $state, $http, $rootScope, exportViewModel, $uibModal) {
    if (exportViewModel.data.IsValid) {
        if (exportViewModel.data.IsAuthorized) {
            initExport();
            initGrid();

            $scope.showExportUpdateBtn = true;
            $scope.showExportUpdateMsg = false;
        }
        else {
            $state.go('unauthorized');
        }
    }
    else {
        $state.go('error');
    }

    function initExport() {
        $scope.exportModel = {};
        $scope.exportModel.Name = exportViewModel.data.Name;
        $scope.exportModel.TicketExportID = exportViewModel.data.TicketExportID;
        $scope.exportModel.Price = exportViewModel.data.Price;
        $scope.exportModel.Capacity = exportViewModel.data.Capacity;
    };

    function initGrid() {
        $scope.ticketExportItemsGridOptions = {
            enableHorizontalScrollbar: 0,
            enableVerticalScrollbar: 2,
            enableRowSelection: true, enableRowHeaderSelection: false,
            columnDefs: [
                         { name: 'ID/Kód', field: 'Code', enableCellEdit: false, width: '15%' },
                         { name: 'Email', field: 'Email', enableCellEdit: true, width: '30%' },
                         { name: 'Jméno', field: 'Name', enableCellEdit: true, width: '30%' },
                         { name: 'Zaplaceno', field: 'Paid', enableCellEdit: true, width: '10%' },
                         { name: 'Odeslat vstupenku', field: 'SendTicketEmail', enableCellEdit: true, width: '15%' }
            ],
        };
        $scope.ticketExportItemsGridOptions.multiSelect = false;
        $scope.ticketExportItemsGridOptions.data = exportViewModel.data.TicketExportItems;
    };

    $scope.ticketExportItemsGridOptions.onRegisterApi = function (gridApi) {
        $scope.ticketItemsGridApi = gridApi;
    };

    $scope.submitExportUpdate = function () {

        $scope.showExportUpdateBtn = false;
        $scope.showExportUpdateMsg = true;

        var exportSubmitModel = {};
        exportSubmitModel.TicketExportID = $scope.exportModel.TicketExportID;
        exportSubmitModel.TicketExportItems = $scope.ticketExportItemsGridOptions.data;

        $http.post('/api/project/tickets/UpdateTicketExport', exportSubmitModel)
        .success(function (data) {
            $scope.showExportUpdateBtn = true;
            $scope.showExportUpdateMsg = false;
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
    };

    function ShowSuccessModal() {
        $scope.successModal = {};
        $scope.successModal.Header = "Uložení změn";
        $scope.successModal.Message = "Změny byly úspěšně uloženy.";
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'http://localhost:44301/AngularViews/Modals/Success.html',
            controller: ModalSuccessCtrl,
            scope: $scope
        });
    };

    function ShowErrorModal(errorMessage) {
        $scope.errorModal = {};
        $scope.errorModal.Header = "Uložení změn";
        $scope.errorModal.Message = errorMessage;
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'http://localhost:44301/AngularViews/Modals/HeaderMessageDanger.html',
            controller: ModalShowError,
            scope: $scope
        });
    }
});

var ModalSuccessCtrl = function ($scope, $uibModalInstance) {
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
};