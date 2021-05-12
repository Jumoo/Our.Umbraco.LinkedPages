using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;

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
using Newtonsoft.Json.Converters;

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


        [HttpPost]
        public IEnumerable<LinkReport> CheckLinks(LinkInfo info)
        {
            var source = Services.ContentService.GetById(info.Source);
            var target = Services.ContentService.GetById(info.Target);

            if (source == null || target == null) throw new KeyNotFoundException();

            var report = new List<LinkReport>();

            var item = new LinkReport
            {
                Path = source.Name,
                Source = source.Id,
                SourceName = source.Name,
                Status = LinkStatus.NoLink,
                Target = target.Id,
                TargetName = target.Name
            };

            item.Status = GetRelationStatus(source, target, target.Path);

            report.Add(item);
            report.AddRange(CheckChildren(source, target, source.Name, target.Path));

            return report;
        }

        private LinkStatus GetRelationStatus(IContent source, IContent target, string rootPath)
        {
            var relations = Services.RelationService.GetByParent(source);

            // if it is filtered. 
            if (relationTypeId != 0) relations = relations.Where(x => x.RelationTypeId == this.relationTypeId);
                
            var direct = relations.FirstOrDefault(x => x.ChildId == target.Id);

            if (direct != null) return LinkStatus.Related;

            if (direct == null)
            {
                foreach(var relation in relations)
                {
                    var targetNode = Services.EntityService.Get(relation.ChildId);
                    if (targetNode.Path.InvariantStartsWith(rootPath))
                    {
                        // a relation somewhere lese on the relation tree. 
                        return LinkStatus.Mismatched;
                    }
                }
            }

            return LinkStatus.NoLink;
        }

        private IEnumerable<LinkReport> CheckChildren(IContent source, IContent target, string path, string rootPath)
        {
            var report = new List<LinkReport>();

            var sourceChildren = Services.ContentService.GetPagedChildren(source.Id, 0, 1000, out long sourceTotal);
            var targetChildren = Services.ContentService.GetPagedChildren(target.Id, 0, 1000, out long targetTotal);

            foreach (var sourceChild in sourceChildren)
            {
                var item = new LinkReport
                {
                    Source = sourceChild.Id,
                    SourceName = sourceChild.Name,
                    Status = LinkStatus.Related,
                    Path = path + "/" + sourceChild.Name                   
                };

                var targetChild = targetChildren.FirstOrDefault(x => x.Name.InvariantEquals(sourceChild.Name));

                if (targetChild != null)
                {
                    item.Target = targetChild.Id;
                    item.TargetName = targetChild.Name;

                    item.Status = GetRelationStatus(sourceChild, targetChild, rootPath);


                    report.AddRange(CheckChildren(sourceChild, targetChild, path + "/" + sourceChild.Name, rootPath));

                }
                else
                {
                    item.TargetName = "(not found)";
                    item.Status = LinkStatus.MissingTarget;
                }

                report.Add(item);
            }

            return report;
        }


        [HttpPost]
        public int FixLinks(LinkFixRequest request)
        {
            int count = 0;

            var rootTarget = Services.ContentService.GetById(request.Target);

            foreach(var link in request.Links)
            {
                var source = Services.ContentService.GetById(link.Source);
                var target = Services.ContentService.GetById(link.Target);

                if (source != null)
                {
                    if (target != null)
                    {
                        if (GetRelationStatus(source, target, rootTarget.Path) == LinkStatus.NoLink)
                        {
                            CreateLink(source.Id, target.Id);
                            count++;
                        }
                    }
                    else
                    {
                        // missing target page ? do we create it
                        // could end up with a lot of duplicates if we do this.
                    }
                }
            }

            return count;
        }

    }

    [JsonObject(NamingStrategyType = typeof(DefaultNamingStrategy))]
    public class LinkInfo
    {
        public int Source { get; set; }
        public int Target { get; set; }
    }

    [JsonObject(NamingStrategyType = typeof(DefaultNamingStrategy))]
    public class LinkReport
    {
        public string Path { get; set; }

        public int Source { get; set; }
        public string SourceName { get; set; }
        public int Target { get; set; }
        public string TargetName { get; set; }

        public LinkStatus Status { get; set; }
      
    }


    [JsonObject(NamingStrategyType = typeof(DefaultNamingStrategy))]
    public class LinkFixRequest
    {
        public int Target { get; set; }
        public IEnumerable<LinkReport> Links { get; set; }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum LinkStatus
    {
        NoLink,
        MissingTarget,
        Mismatched,
        Related = 100
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
