(function () {

    'use strict';

    function linkedDialogController(
        $scope, $q,
        navigationService,
        notificationsService,
        editorService,
        linkedPageService) {

        var vm = this;

        vm.loaded = false;
        vm.nodeId = $scope.currentNode.id;
        vm.nodeName = $scope.currentNode.name;

        vm.showType = Umbraco.Sys.ServerVariables.LinkedPages.showRelationType;
        vm.typeAlias = Umbraco.Sys.ServerVariables.LinkedPages.relationTypeAlias;
        vm.error = '';

        vm.children = [];
        vm.parents = [];

        vm.relationCount = 0;

        vm.removeLink = removeLink;
        vm.addLink = addLink;

        vm.close = close;

        Init();

        //////////////////

        function addLink() {
            console.log('add link');

            editorService.contentPicker({
                multiPicker: false,
                submit: function (model) {
                    console.log(model);
                    linkedPageService.createLink(vm.nodeId, model.selection[0].id)
                        .then(function (result) {
                            vm.children = result.data;
                            vm.relationCount = vm.children.length + vm.parents.length;
                        }, function (error) {
                            vm.error = error.data.ExceptionMessage;
                        });
                    editorService.close();
                },
                close: function () {
                    editorService.close();
                }
            });
        }

        function removeLink(id) {
            console.log('remove link', id);

            linkedPageService.removeLink(id, vm.nodeId)
                .then(function (result) {
                    vm.children = result.data;
                    vm.relationCount = vm.children.length + vm.parents.length;
                }, function (error) {
                    vm.error = error.data.ExceptionMessage;
                });
        }

        //////////////////
        function getChildren(promiseArray, id) {

            promiseArray.push(
                linkedPageService.getChildren(id)
                    .then(function (result) {
                        vm.children = result.data;
                    }, function (error) {
                        vm.error = error.data.ExceptionMessage;
                    })
            );
        }


        function getParents(promiseArray, id) {

            promiseArray.push(
                linkedPageService.getParents(id)
                    .then(function (result) {
                        vm.parents = result.data;
                    }, function (error) {
                        vm.error = error.data.ExceptionMessage;
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

        function close() {
            navigationService.hideDialog();
        }
    }

    angular.module('umbraco')
        .controller("linkedPageDialogController", linkedDialogController);

})();