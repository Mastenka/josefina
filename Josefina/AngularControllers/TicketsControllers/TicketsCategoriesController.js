JosefinaApp.controller('TicketsCategoriesController', function TicketsCategoriesController($scope, $state, $http, $rootScope, ticketsCategoriesViewModel, i18nService, uiGridConstants, $uibModal) {
    i18nService.setCurrentLang('cz');
    if (ticketsCategoriesViewModel.data.IsValid) {
        if (ticketsCategoriesViewModel.data.IsAuthorized) {
            //Category grid
            $scope.categoriesGridOptions = {
                enableHorizontalScrollbar: 0,
                enableVerticalScrollbar: 2,
                enableRowSelection: true, enableRowHeaderSelection: false,
                showColumnFooter: true,
                columnDefs: [
                             { name: 'Název', field: 'Name' },
                             { name: 'Celkový příjem', field: 'PaidTotal', aggregationType: uiGridConstants.aggregationTypes.sum },
                             { name: 'Zaplacených', field: 'Paid', aggregationType: uiGridConstants.aggregationTypes.sum },
                             { name: 'Nezaplacených', field: 'Unpaid', aggregationType: uiGridConstants.aggregationTypes.sum },
                             { name: 'Počet platných vstupenek', field: 'ReallyPaid', aggregationType: uiGridConstants.aggregationTypes.sum },
                             { name: 'Kapacita', field: 'Capacity', aggregationType: uiGridConstants.aggregationTypes.sum },
                             { name: 'Cena', field: 'TicketPrice' },
                             { name: 'Začátek prodeje', field: 'SoldFrom', cellFilter: 'date:\'dd.MM.yyyy\'' },
                             { name: 'Konec prodeje', field: 'SoldTo', cellFilter: 'date:\'dd.MM.yyyy\'' },
                ],
            };
            $scope.categoriesGridOptions.multiSelect = false;
            $scope.categoriesGridOptions.data = ticketsCategoriesViewModel.data.Categories;

            $rootScope.PageTitle = ticketsCategoriesViewModel.data.Title;
            $scope.ticketCategoriesActive = true;

        }
        else {
            $state.go('unauthorized');
        }
    }
    else {
        $state.go('error');
    }

    $scope.categoriesGridOptions.onRegisterApi = function (gridApi) {
        $scope.categoriesGridApi = gridApi;
    };

    $scope.UpdateCategory = function () {
        var selectedRow = $scope.categoriesGridApi.selection.getSelectedRows();

        if (selectedRow.length === 0) {
            $scope.errorModal = { Header: "Editace", Message: "Není vybrána položka k editaci." }

            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: 'http://localhost:44301/AngularViews/Modals/HeaderMessageDanger.html',
                controller: ModalShowError,
                scope: $scope
            });
        }
        else {

            if (selectedRow[0].IsCategory)
            {
                var soldFromDT = Date.parse(selectedRow[0].SoldFrom);
                var soldFromT = new Date(soldFromDT);
                var soldToDT = Date.parse(selectedRow[0].SoldTo);

                $scope.categoryModal = {
                    header: "Editace",
                    name: "",
                    button: "Uložit",
                    name: selectedRow[0].Name,
                    capacity: selectedRow[0].Capacity,
                    price: selectedRow[0].TicketPrice,
                    soldFrom: soldFromDT,
                    soldFromTime: soldFromT.getHours() + ':' + soldFromT.getMinutes(),
                    soldTo: soldToDT,
                    codeRequired: selectedRow[0].CodeRequired,
                    code: selectedRow[0].Code,
                    isNew: false,
                    categoryID: selectedRow[0].TicketCategoryID
                };

                var modalInstance = $uibModal.open({
                    animation: true,
                    templateUrl: 'http://localhost:44301/AngularViews/Modals/TicketCategory.html',
                    controller: ModalCreateUpdateCategoryCtrl,
                    scope: $scope
                });
            }
            else
            {
                $state.go('tickets.export.edit', { exportID: selectedRow[0].TicketCategoryID });
            }
        }
    };

    $scope.DeleteCategory = function () {
        var selectedRow = $scope.categoriesGridApi.selection.getSelectedRows();

        if (selectedRow.length === 0) {
            $scope.errorModal = { Header: "Odstranění kategorie", Message: "Není vybrána kategorie ke smazání." }

            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: 'http://localhost:44301/AngularViews/Modals/HeaderMessageDanger.html',
                controller: ModalShowError,
                scope: $scope
            });
        }
        else {

            if (selectedRow[0].Paid > 0 || selectedRow[0].Unpaid > 0) {
                $scope.confirmationModal = { Header: "Odstranění kategorie obsahující vstupenky", Message: "Daná kategorie obsahuje prodané nebo rezervované vstupenky. Opravdu si ji přejete přesto smazat? (Existující vstupenky nebudou smazány)" };
            }
            else {
                $scope.confirmationModal = { Header: "Odstranění kategorie", Message: "Opravdu si přejete danou kategorii samazat?" };
            }
            $scope.confirmationModal.button1 = "Ano";
            $scope.confirmationModal.button2 = "Ne";
            $scope.confirmationModal.NodeId = selectedRow[0].TicketCategoryID;
            var modalInstance = $uibModal.open({
                animation: true,
                templateUrl: 'http://localhost:44301/AngularViews/Modals/Confirmation.html',
                controller: ModalDeleteCtrl,
                scope: $scope
            });

        }

    };

    $scope.NewCategory = function () {
        $scope.categoryModal = {
            header: "Nová kategorie vstupenek",
            name: "", button: "Vytvořit",
            isNew: true
        };
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'http://localhost:44301/AngularViews/Modals/TicketCategory.html',
            controller: ModalCreateUpdateCategoryCtrl,
            scope: $scope
        });
    };

    $scope.NewExport = function () {
        $scope.exportModal = {
            header: "Nový export vstupenek",
            name: "", button: "Vytvořit",
            isNew: true
        };
        var modalInstance = $uibModal.open({
            animation: true,
            templateUrl: 'http://localhost:44301/AngularViews/Modals/TicketExport.html',
            controller: ModalCreateExportCtrl,
            scope: $scope
        });
    };

    $scope.$on('createUpdateCategory', function (event, categoryData) {
        console.log(categoryData);
        categoryData.ProjectID = $rootScope.projectID;
        $http.post('/api/project/tickets/CreateUpdateCategory', categoryData)
        .success(function (data) {
            if (data.IsValid) {
                if (data.IsAuthorized) {
                    $scope.categoriesGridOptions.data = data.Categories;
                    $scope.categoriesGridApi.core.refresh();
                }
                else {
                    $state.go('unauthorized');
                }
            }
            else {
                $state.go('error');
            }
        });
    });

    $scope.$on('createExport', function (event, exportModel) {
        exportModel.ProjectID = $rootScope.projectID;
        $http.post('/api/project/tickets/CreateExport', exportModel)
        .success(function (data) {
            if (data.IsValid) {
                if (data.IsAuthorized) {
                    $scope.categoriesGridOptions.data = data.Categories;
                    $scope.categoriesGridApi.core.refresh();
                }
                else {
                    $state.go('unauthorized');
                }
            }
            else {
                $state.go('error');
            }
        });
    });

    $scope.$on('DeleteCategory', function (event, nodeId) {
        var categoryData = { ProjectID: $rootScope.projectID, CategoryID: nodeId };
        $http.post('/api/project/tickets/DeleteCategory', categoryData)
            .success(function (data) {
                if (data.IsValid) {
                    if (data.IsAuthorized) {
                        $scope.categoriesGridOptions.data = data.Categories;
                        $scope.categoriesGridApi.core.refresh();
                    }
                    else {
                        $state.go('unauthorized');
                    }
                }
                else {
                    $state.go('error');
                }
            })
    });
});

var ModalShowError = function ($scope, $uibModalInstance) {
    $scope.cancel = function () {
        $uibModalInstance.dismiss('cancel');
    };
};

var ModalCreateUpdateCategoryCtrl = function ($scope, $uibModalInstance) {
    $scope.categoryModal.soldFromOpen = false;
    $scope.categoryModal.soldToOpen = false;
    $scope.categoryModal.showIntervalError = false;

    var ignoreSubmit = false;

    $scope.submitForm = function () {
        if (ignoreSubmit) {
            return;
        }

        if ($scope.categoryModal.soldTo >= $scope.categoryModal.soldFrom) {
            console.log($scope.categoryModal.soldFrom);
            console.log($scope.categoryModal.soldFromTime);

            var categoryModel = {
                Name: $scope.categoryModal.name,
                Capacity: $scope.categoryModal.capacity,
                Price: $scope.categoryModal.price,
                SoldFrom: new Date($scope.categoryModal.soldFrom),
                SoldTo: new Date($scope.categoryModal.soldTo),
                CategoryID: $scope.categoryModal.categoryID,
                IsNew: $scope.categoryModal.isNew,
                CodeRequired: $scope.categoryModal.codeRequired,
                Code: $scope.categoryModal.code
            };
 
            categoryModel.SoldFrom.setHours($scope.categoryModal.soldFromTime.split(':')[0], $scope.categoryModal.soldFromTime.split(':')[1]);
            console.log(categoryModel.SoldFrom);

            $scope.$emit('createUpdateCategory', categoryModel);
            $uibModalInstance.close('closed');
        }
        else {
            $scope.categoryModal.showIntervalError = true;
        }
    };

    $scope.soldFromClick = function () {
        $scope.categoryModal.soldFromOpen = !$scope.categoryModal.soldFromOpen;
    };

    $scope.soldToClick = function () {
        $scope.categoryModal.soldToOpen = !$scope.categoryModal.soldToOpen;
    };

    $scope.cancel = function () {
        ignoreSubmit = true;
        $uibModalInstance.close('closed');
    };
};

var ModalCreateExportCtrl = function ($scope, $uibModalInstance) {
    var ignoreSubmit = false;

    $scope.submitForm = function () {
        if (ignoreSubmit) {
            return;
        }

        var exportModel = {
            Name: $scope.exportModal.name,
            Capacity: $scope.exportModal.capacity,
            Price: $scope.exportModal.price
        };

        $scope.$emit('createExport', exportModel);
        $uibModalInstance.close('closed');

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
        $scope.$emit('DeleteCategory', $scope.confirmationModal.NodeId);
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