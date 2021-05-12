(function () {
    'use strict'

    function checkerController($scope, linkedPageService) {

        var vm = this;

        vm.toggleSelect = toggleSelect;
        vm.selectItem = selectItem;

        vm.onTreeInit = onTreeInit;
        vm.check = check;
        vm.fix = fix;

        vm.checkbuttonState = 'init';
        vm.report = [];

        vm.sourceTree = {
            dialogTreeApi: {}
        };

        vm.targetTree = {
            dialogTreeApi: {}
        }

        function onTreeInit(tree) {
            tree.dialogTreeApi.callbacks.treeNodeSelect(function (args) {
                nodeSelectedHandler(tree, args)
            });
        } 

        function nodeSelectedHandler(tree, args) {

            if (args && args.event) {
                args.event.preventDefault();
                args.event.stopPropagation();
            }

            if (tree.target) {
                tree.target.selected = false;
            }

            tree.target = args.node;
            tree.target.selected = true;
        }

        function check() {

            vm.checkbuttonState = 'busy';

            linkedPageService.checkLinks(vm.sourceTree.target.id, vm.targetTree.target.id)
                .then(function (result) {
                    vm.report = result.data;
                    selectAll(vm.report, true);
                    vm.checkbuttonState = 'success';
                });
        }

        function fix() {
            vm.checkbuttonState = 'busy';

            linkedPageService.fixLinks(vm.targetTree.target.id, vm.selectedItems)
                .then(function (result) {
                    vm.report = [];
                    vm.checkbuttonState = 'success';
                });

        }

        function toggleSelect() {
            vm.selected = !vm.selected;
            selectAll(vm.report, vm.selected);
        }

        function selectItem(item, $event) {



            item.selected = !item.selected;

            getSelected();
        }


        function selectAll(items, selected) {
            vm.selected = selected;
            _.each(items, function (item) {
                if (item.Status == 'NoLink') {
                    item.selected = selected;
                }
            });

            getSelected();
        }

        vm.selectedItems = [];

        function getSelected() {
            vm.selectedItems = [];

            _.each(vm.report, function (item) {
                if (item.selected) {
                    vm.selectedItems.push(item);
                }
            });
        }
    }

    angular.module('umbraco')
        .controller('linkedpagesCheckerController', checkerController);
})();