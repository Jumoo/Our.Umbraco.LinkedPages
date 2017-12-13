(function () {

    'use strict';

    function linkedDialogController(
        $scope, $q,
        navigationService,
        dialogService,
        linkedPageService) {

        var vm = this;

        vm.loaded = false;
        vm.nodeId = $scope.currentNode.id;
        vm.nodeName = $scope.currentNode.name;

        vm.children = [];
        vm.parents = [];

        vm.relationCount = 0;

        vm.removeLink = removeLink;
        vm.addLink = addLink;

        Init();

        //////////////////

        function addLink() {
            console.log('add link');

            dialogService.contentPicker({
                multiPicker: false,
                callback: function (data) {
                    console.log(data);
                    linkedPageService.createLink(vm.nodeId, data.id)
                        .then(function (result) {
                            vm.children = result.data;
                            vm.relationCount = vm.children.length + vm.parents.length; 
                        });
                }
            })
        }

        function removeLink(id) {
            console.log('remove link', id);

            linkedPageService.removeLink(id, vm.nodeId)
                .then(function (result) {
                    vm.children = result.data;
                    vm.relationCount = vm.children.length + vm.parents.length; 
                });
        }

        //////////////////
        function getChildren(promiseArray, id) {

            promiseArray.push(
                linkedPageService.getChildren(id)
                    .then(function (result) {
                        vm.children = result.data;

                    }, function (error) {
                        // error
                    })
            );
        }


        function getParents(promiseArray, id) {

            promiseArray.push(
                linkedPageService.getParents(id)
                    .then(function (result) {
                        vm.parents = result.data;
                    })
            );
        }

        function Init() {

            var promises = [];

            getChildren(promises, $scope.currentNode.id);
            getParents(promises, $scope.currentNode.id);

            $q.all(promises)
                .then(function () {
                    vm.relationCount = vm.parents.length + vm.children.length;
                    vm.loaded = true;
                });
        }



    }

    angular.module('umbraco')
        .controller("linkedPageDialogController", linkedDialogController);

})();