JosefinaApp.config(function ($stateProvider, $urlRouterProvider) {

    $urlRouterProvider.otherwise('/tasks');

    $stateProvider
      .state('tasks', {
          url: '/tasks',
          views: {
              // layout 
              '': { templateUrl: 'http://localhost:44301/AngularViews/LayoutViews/3PanelsLayout.html' },
              // TreeView
              'treeView@tasks': {
                  templateUrl: 'http://localhost:44301/AngularViews/TreeView.html',
                  controller: 'TasksTreeViewController'
              }
          }
      })
      .state('tasks.error', {
          url: '/error',
          views: {
              'rightView': {
                  templateUrl: 'http://localhost:44301/AngularViews/Error.html'
              }
          }
      })
      .state('tasks.unauthorized', {
          url: '/unauthorized',
          views: {
              'rightView': {
                  templateUrl: 'http://localhost:44301/AngularViews/Unauthorized.html'
              }
          }
      })
      .state('tasks.task', {
          url: '/task',
          views: {
              //RightPanel with default info              
              'rightView': {
                  templateUrl: 'http://localhost:44301/AngularViews/LayoutViews/2PanelsRightLayout.html'
              }
          }
      })
      .state('tasks.task.detail', {
          url: '/:taskId',
          resolve: {
              taskViewModel: ['$http', '$stateParams', '$location', function ($http, $stateParams, $location) {
                  var taskId = "";
                  if ($stateParams.taskId === "" || $stateParams.taskId === undefined) {
                      taskId = $location.path().split('/')[3];
                  }
                  else {
                      taskId = $stateParams.taskId;
                  }
                  return $http({
                      method: 'GET',
                      url: '/api/project/tasks/gettaskviewmodel/' + taskId,
                  });
              }]
          },
          views: {
              // Top rigtht: Content
              'upperView': {
                  templateUrl: 'http://localhost:44301/AngularViews/UpperViews/Tasks/TaskContentView.html',
                  controller: 'TaskUpperController'
              },
              // Bottom right: Comments
              'lowerView': {
                  templateUrl: 'http://localhost:44301/AngularViews/LowerViews/Tasks/TaskLowerView.html',
                  controller: 'TaskLowerController'
              },
              // Bottom left: Properties
              'propertiesView@tasks': {
                  templateUrl: 'http://localhost:44301/AngularViews/PropertiesViews/Tasks/TaskPropertiesView.html',
                  controller: 'TaskPropertiesController'
              },
          }
      }).state('tasks.task.detail.edit', {
          url: '/:taskId',
          views: {
              // Top rigtht: Content
              'upperView@tasks.task': {
                  templateUrl: 'http://localhost:44301/AngularViews/UpperViews/Tasks/TaskEditContentView.html',
                  controller: 'TaskEditUpperController'
              },
              // Bottom left: Properties
              'propertiesView@tasks': {
                  templateUrl: 'http://localhost:44301/AngularViews/PropertiesViews/Tasks/TaskEditPropertiesView.html',
                  controller: 'TaskEditPropertiesController'
              },
          }
      }).state('tickets', {
          url: '/tickets',
          templateUrl: 'http://localhost:44301/AngularViews/TicketsViews/TicketsMain.html',
      }).state('tickets.ticketSettings', {
          url: '/settings',
          templateUrl: 'http://localhost:44301/AngularViews/TicketsViews/TicketsSettings.html'
      }).state('tickets.ticketSettings.bank', {
          url: '/bank',
          templateUrl: 'http://localhost:44301/AngularViews/TicketsViews/TicketsBankSettings.html',
          controller: 'TicketsBankSettingsController',
          resolve: {
              ticketsBankSettingsViewModel: ['$http', '$rootScope', function ($http, $rootScope) {
                  return $http({
                      method: 'GET',
                      url: '/api/project/tickets/GetBankSettingsViewModel/' + $rootScope.projectID,
                  });
              }]
          }
      }).state('tickets.ticketSettings.tickets', {
          url: '/ticket',
          templateUrl: 'http://localhost:44301/AngularViews/TicketsViews/TicketsTicketSettings.html',
          controller: 'TicketsTicketSettingsController',
          resolve: {
              ticketsTicketSettingsViewModel: ['$http', '$rootScope', function ($http, $rootScope) {
                  return $http({
                      method: 'GET',
                      url: '/api/project/tickets/GetTicketSettingsViewModel/' + $rootScope.projectID,
                  });
              }]
          }
      }).state('tickets.categories', {
          url: '/categories',
          templateUrl: 'http://localhost:44301/AngularViews/TicketsViews/TicketsCategories.html',
          controller: 'TicketsCategoriesController',
          resolve: {
              ticketsCategoriesViewModel: ['$http', '$rootScope', function ($http, $rootScope) {
                  return $http({
                      method: 'GET',
                      url: '/api/project/tickets/GetCategoriesViewModel/' + $rootScope.projectID,
                  });
              }]
          }
      }).state('tickets.orders', {
          url: '/orders',
          templateUrl: 'http://localhost:44301/AngularViews/TicketsViews/TicketsOrders.html',
          controller: 'TicketsOrdersController',
          resolve: {
              ticketsOrdersViewModel: ['$http', '$rootScope', function ($http, $rootScope) {
                  return $http({
                      method: 'GET',
                      url: '/api/project/tickets/GetOrdersViewModel/' + $rootScope.projectID,
                  });
              }]
          }
      }).state('tickets.order', {
          url: '/:orderID',
          templateUrl: 'http://localhost:44301/AngularViews/TicketsViews/TicketOrder.html',
          controller: 'TicketsOrderController',
          resolve: {
              orderViewModel: ['$http', '$stateParams', '$location', function ($http, $stateParams, $location) {
                  var orderID = "";
                  if ($stateParams.orderID === "" || $stateParams.orderID === undefined) {
                      orderID = $location.path().split('/')[2];
                  }
                  else {
                      orderID = $stateParams.orderID;
                  }
                  return $http({
                      method: 'GET',
                      url: '/api/project/tickets/GetOrderViewModel/' + orderID,
                  });
              }]
          }
      }).state('tickets.export', {
          url: '/export',
          template: '<div ui-view></div>', //TODO
      }).state('tickets.export.edit', {
          url: '/:exportID',
          templateUrl: 'http://localhost:44301/AngularViews/TicketsViews/TicketExport.html',
          controller: 'TicketsExportController',
          resolve: {
              exportViewModel: ['$http', '$stateParams', '$location', function ($http, $stateParams, $location) {
                  var exportID = "";
                  if ($stateParams.exportID === "" || $stateParams.exportID === undefined) {
                      exportID = $location.path().split('/')[2];
                  }
                  else {
                      exportID = $stateParams.exportID;
                  }
                  return $http({
                      method: 'GET',
                      url: '/api/project/tickets/GetExportViewModel/' + exportID,
                  });
              }]
          }
      });
});