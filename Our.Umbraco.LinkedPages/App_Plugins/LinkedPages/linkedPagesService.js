(function () {

    'use strict';

    function linkedPageService($http) {

        var serviceRoot = Umbraco.Sys.ServerVariables.LinkedPages.LinkedPageApi;

        var service = {
            getChildren: getChildren,
            getParents: getParents,
            createLink: createLink,
            removeLink: removeLink
        };

        return service;

        /////////////////


        function getChildren(id) {
            return $http.get(serviceRoot + "GetChildLinks/" + id);
        }

        function getParents(id) {
            return $http.get(serviceRoot + "GetParentLinks/" + id);
        }

        function createLink(parent, child) {
            return $http.post(serviceRoot + "CreateLink?parent=" + parent + "&child=" + child);
        }

        function removeLink(id, pageId) {
            return $http.delete(serviceRoot + "RemoveLink/" + id + "?currentPage=" + pageId);
        }

    }

    angular.module('umbraco.resources')
        .factory('linkedPageService', linkedPageService);

})();