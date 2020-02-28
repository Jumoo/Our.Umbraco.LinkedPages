using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Http;

using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Models.Entities;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace Our.Umbraco.LinkedPages.Controllers
{
    [PluginController("LinkedPages")]
    public class LinkedPagesApiController : UmbracoAuthorizedApiController
    {
        private readonly string defaultRelationType = Constants.Conventions.RelationTypes.RelateDocumentOnCopyAlias;
        private readonly int relationTypeId = 0;

        public LinkedPagesApiController()
        {
            var relationTypeAlias = ConfigurationManager.AppSettings["LinkedPages.RelationType"];
            if (!string.IsNullOrWhiteSpace(relationTypeAlias))
            {
                this.defaultRelationType = relationTypeAlias;

                var relationType = Services.RelationService.GetRelationTypeByAlias(relationTypeAlias);
                if (relationType != null)
                {
                    this.relationTypeId = relationType.Id;
                }
            }

        }

        /// <summary>
        ///  simple end point - used to get the URL for the Api 
        /// </summary>
        [HttpGet]
        public bool GetApi()
            => true;


        [HttpGet]
        public IEnumerable<LinkedPageInfo> GetChildLinks(int id)
        {
            var relations = Services.RelationService.GetByParentId(id);
            if (!relations.Any())
                return Enumerable.Empty<LinkedPageInfo>();

            return GetRelations(relations, true);
        }

        [HttpGet]
        public IEnumerable<LinkedPageInfo> GetParentLinks(int id)
        {
            var relations = Services.RelationService.GetByChildId(id);
            if (!relations.Any())
                return Enumerable.Empty<LinkedPageInfo>();

            return GetRelations(relations, false);
        }

        [HttpPost]
        public IEnumerable<LinkedPageInfo> CreateLink(int parent, int child)
        {
            var parentNode = Services.EntityService.Get(parent);
            var childNode = Services.EntityService.Get(child);

            if (parentNode == null || childNode == null)
                throw new KeyNotFoundException();

            var relationType = Services.RelationService.GetRelationTypeByAlias(defaultRelationType);
            if (relationType == null)
                throw new ApplicationException($"Cannot create relation of type {defaultRelationType}");

            var relation = new Relation(parent, child, relationType);
            Services.RelationService.Save(relation);

            return GetChildLinks(parent);
        }

        [HttpDelete]
        public IEnumerable<LinkedPageInfo> RemoveLink(int id, int currentPage)
        {
            var relation = Services.RelationService.GetById(id);
            if (relation == null)
                throw new ArgumentOutOfRangeException($"Cannot find relation with id {id}");
            
            Services.RelationService.Delete(relation);

            return GetChildLinks(currentPage);
        }

        private IEnumerable<LinkedPageInfo> GetRelations(IEnumerable<IRelation> relations, bool linkChild)
        { 
            foreach (var relation in relations)
            {
                if (relationTypeId == 0 ||
                    relation.RelationTypeId == this.relationTypeId)
                {
                    var nodeId = linkChild ? relation.ChildId : relation.ParentId;
                    var node = Services.EntityService.Get(nodeId);
                    if (node == null) continue;

                    yield return new LinkedPageInfo
                    {
                        RelationId = relation.Id,
                        PageId = nodeId,
                        Name = node.Name,
                        Path = GetContentPath(node),
                        RelationType = relation.RelationType.Alias,
                        RelationTypeId = relation.RelationTypeId
                    };
                }
            }
        }


        private string GetContentPath(IEntitySlim item)
        {
            if (item == null) return string.Empty;

            var path = string.Empty;
            if (item.ParentId > -1)
            {
                var parent = Services.EntityService.GetParent(item.Id);
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
        public int RelationTypeId { get; set; }
        public string RelationType { get; set; }
    }

}
