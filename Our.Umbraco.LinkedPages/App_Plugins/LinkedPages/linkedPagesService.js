(function () {

    'use strict';

    function linkedPageService($http) {

        var serviceRoot = Umbraco.Sys.ServerVariables.LinkedPages.LinkedPageApi;

        var service = {
            getChildren: getChildren,
            getParents: getParents,
            createLink: createLink,
            removeLink: removeLink,

            checkLinks: checkLinks,
            fixLinks: fixLinks
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

        function checkLinks(source, target) {
            return $http.post(serviceRoot + "CheckLinks", { source: source, target: target });
        }

        function fixLinks(target, links) {
            return $http.post(serviceRoot + "FixLinks", { target: target, links: links });
        }

    }

    angular.module('umbraco.resources')
        .factory('linkedPageService', linkedPageService);

})();