/* Nutné nastavit lastHeight a lastWidth v controlleru v kterém je direktiva použita
 */

JosefinaApp.directive('setHeightDock', function () {
    function link($scope, element, attrs) { //scope we are in, element we are bound to, attrs of that element
        $scope.$watch(function () { //watch any changes to our element
            var refresh = false;
            var currentWidth = element[0].offsetWidth;
            var currentHeight = element[0].offsetHeight;

            if ($scope.setDockData === undefined) {
                refresh = true;
                $scope.setDockData = { LastHeight: "", LastWidth: "", Style: "" };
            }
            else
            {
                refresh = currentHeight !== $scope.setDockData.LastHeight || currentWidth !== $scope.setDockData.LastWidth;
            }
            
            if (refresh) {
                $scope.setDockData.LastHeight = currentHeight;
                $scope.setDockData.LastWidth = currentWidth;

                $scope.setDockData.Style = { //scope variable style, shared with our controller
                    height: currentHeight + 'px', //set the height in style to our elements height
                    width: currentWidth + 'px', //same with width - 2
                  
                };
            }
        });
    }
    return {
        restrict: 'AE', //describes how we can assign an element to our directive in this case like <div master></div
        link: link // the function to link to our element
    };
});