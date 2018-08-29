using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Umbraco.Core.Models;
using Umbraco.Core.Models.EntityBase;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Our.Umbraco.LinkedPages.Controllers
{
    [PluginController("LinkedPages")]
    public class LinkedPagesApiController : UmbracoAuthorizedApiController
    {
        private string defaultRelationType;
        private int relationTypeId = 0;

        public LinkedPagesApiController()
        {
            var type = ConfigurationManager.AppSettings["LinkedPages.RelationType"];
            if (string.IsNullOrWhiteSpace(type))
            {
                defaultRelationType = "relateDocumentOnCopy";
            }
            else
            {
                defaultRelationType = type;
                var relationType = Services.RelationService.GetRelationTypeByAlias(type);
                if (relationType != null)
                {
                    relationTypeId = relationType.Id;
                }
            }
        }

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
                if (relationTypeId == 0 || relation.RelationTypeId == relationTypeId)
                {

                    var node = Services.EntityService.Get(relation.ChildId);
                    if (node == null)
                        continue;

                    var pageInfo = new LinkedPageInfo()
                    {
                        RelationId = relation.Id,
                        PageId = node.Id,
                        Name = node.Name,
                        Path = GetContentPath(node),
                        RelationType = relation.RelationType.Alias,
                        RelationTypeId = relation.RelationTypeId
                    };

                    links.Add(pageInfo);
                }
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
                if (relationTypeId == 0 || relation.RelationTypeId == relationTypeId)
                {
                    var node = Services.EntityService.Get(relation.ParentId);
                    if (node == null)
                        continue;

                    var pageInfo = new LinkedPageInfo()
                    {
                        RelationId = relation.Id,
                        PageId = node.Id,
                        Path = GetContentPath(node),
                        Name = node.Name,
                        RelationType = relation.RelationType.Alias,
                        RelationTypeId = relation.RelationTypeId
                    };

                    links.Add(pageInfo);
                }
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

            var relationType = Services.RelationService.GetRelationTypeByAlias(defaultRelationType);
            if (relationType != null)
            {
                var relation = new Relation(parent, child, relationType);
                Services.RelationService.Save(relation);
            }
            else
            {
                throw new ApplicationException($"Cannot create new relation of type {defaultRelationType}");
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

        private string GetContentPath(IUmbracoEntity item)
        {
            if (item == null)
                return "";

            var path = string.Empty;
            if (item.ParentId > -1)
            {
                var parent = Services.EntityService.Get(item.ParentId);
                if (parent != null)
                    path += GetContentPath(parent);
            }

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

        //
        public int RelationTypeId { get; set; }
        public string RelationType { get; set; }
    }
}
