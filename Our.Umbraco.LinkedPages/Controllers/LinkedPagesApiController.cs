using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Umbraco.Core.Models;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Our.Umbraco.LinkedPages.Controllers
{
    [PluginController("LinkedPages")]
    public class LinkedPagesApiController : UmbracoAuthorizedApiController
    {

        /// <summary>
        ///  returns true, used to get the URL of the controller,
        ///  when registering the constant in the script.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public bool GetApiController()
        {
            return true;
        }

        [HttpGet]
        public IEnumerable<LinkedPageInfo> GetChildLinks(int id)
        {
            var relations = Services.RelationService.GetByParentId(id);
            if (!relations.Any())
                return Enumerable.Empty<LinkedPageInfo>();

            var links = new List<LinkedPageInfo>();

            foreach (var relation in relations)
            {

                var node = Services.ContentService.GetById(relation.ChildId);
                if (node == null)
                    continue;

                var pageInfo = new LinkedPageInfo()
                {
                    RelationId = relation.Id,
                    PageId = node.Id,
                    Name = node.Name,
                    Path = GetContentPath(node)
                };

                links.Add(pageInfo);
            }

            return links;
        }

        [HttpGet]
        public IEnumerable<LinkedPageInfo> GetParentLinks(int id)
        {
            var relations = Services.RelationService.GetByChildId(id);
            if (!relations.Any())
                return Enumerable.Empty<LinkedPageInfo>();

            var links = new List<LinkedPageInfo>();

            foreach(var relation in relations)
            {
                var node = Services.ContentService.GetById(relation.ParentId);
                if (node == null)
                    continue;

                var pageInfo = new LinkedPageInfo()
                {
                    RelationId = relation.Id,
                    PageId = node.Id,
                    Path = GetContentPath(node),
                    Name = node.Name
                };

                links.Add(pageInfo);
            }

            return links;

        }


        [HttpPost]
        public IEnumerable<LinkedPageInfo> CreateLink(int parent, int child)
        {

            var parentNode = Services.ContentService.GetById(parent);
            var childNode = Services.ContentService.GetById(child);

            if (parentNode == null || childNode == null)
                return GetChildLinks(parent);

            var relationType = Services.RelationService.GetRelationTypeByAlias("relateDocumentOnCopy");
            if (relationType != null)
            {
                var relation = new Relation(parent, child, relationType);

                Services.RelationService.Save(relation);

            }

            return GetChildLinks(parent);
        }

        [HttpDelete]
        public IEnumerable<LinkedPageInfo> RemoveLink(int id, int currentPage)
        {
            var relation = Services.RelationService.GetById(id);
            if (relation != null)
            {
                Services.RelationService.Delete(relation);
            }
            return GetChildLinks(currentPage);

        }

        private string GetContentPath(IContent item)
        {
            if (item == null)
                return "";

            var path = string.Empty;
            var parent = item.Parent();
            if (parent != null)
                path += GetContentPath(parent);

            if (!string.IsNullOrWhiteSpace(path))
                return path + " > " + item.Name;

            return item.Name;
        }

    }

    public class LinkedPageInfo
    {
        public int RelationId { get; set; }
        public int PageId { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }
    }
}
