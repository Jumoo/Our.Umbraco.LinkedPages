using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Mvc;

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Models.Entities;
using Umbraco.Cms.Core.Services;
using Umbraco.Cms.Web.BackOffice.Controllers;
using Umbraco.Extensions;

namespace Our.Umbraco.LinkedPages.Controllers;

public class LinkedPagesApiController : UmbracoAuthorizedJsonController
{
    private readonly IRelationService _relationService;
    private readonly IEntityService _entityService;
    private readonly LinkedPagesConfig _config;

    private string defaultRelationType = Constants.Conventions.RelationTypes.RelateDocumentOnCopyAlias;
    private int relationTypeId = 0;
    private int[] _ignoredTypeIds;

    public LinkedPagesApiController(
        IRelationService relationService,
        IEntityService entityService,
        LinkedPagesConfig config)
    {
        _relationService = relationService;
        _entityService = entityService;
        _config = config;

        _ignoredTypeIds = GetIgnoredTypeIds();
    }

    private int[] GetIgnoredTypeIds()
    {
        var ignore = _config.ignoredTypes.ToDelimitedList();
        var types = _relationService.GetAllRelationTypes();

        return types.Where(x => ignore.InvariantContains(x.Alias))
            .Select(x => x.Id)
            .ToArray();
    }

    /// <summary>
    ///  API endpoint - used for discovery in ServerVariablesParser.
    /// </summary>
    [HttpGet]
    public bool GetApi() => true;


    [HttpGet]
    public IEnumerable<LinkedPageInfo> GetChildLinks(int id)
    {
        var relations = _relationService.GetByParentId(id);
        if (!relations.Any())
            return Enumerable.Empty<LinkedPageInfo>();

        return GetRelations(relations, true);
    }

    [HttpGet]
    public IEnumerable<LinkedPageInfo> GetParentLinks(int id)
    {
        var relations = _relationService.GetByChildId(id);
        if (!relations.Any())
            return Enumerable.Empty<LinkedPageInfo>();

        return GetRelations(relations, false);
    }

    [HttpPost]
    public IEnumerable<LinkedPageInfo> CreateLink(int parent, int child)
    {
        var parentNode = _entityService.Get(parent);
        var childNode = _entityService.Get(child);

        if (parentNode == null || childNode == null)
            throw new KeyNotFoundException();
        var typeAlias = string.IsNullOrWhiteSpace(_config.RelationType) ?
                defaultRelationType : _config.RelationType;
        var relationType = _relationService.GetRelationTypeByAlias(typeAlias);
        if (relationType == null)
            throw new ApplicationException($"Cannot create relation of type {typeAlias}");

        var relation = new Relation(parent, child, relationType);
        _relationService.Save(relation);

        return GetChildLinks(parent);
    }

    [HttpDelete]
    public IEnumerable<LinkedPageInfo> RemoveLink(int id, int currentPage)
    {
        var relation = _relationService.GetById(id);
        if (relation == null)
            throw new ArgumentOutOfRangeException($"Cannot find relation with id {id}");

        _relationService.Delete(relation);

        return GetChildLinks(currentPage);
    }


    private IEnumerable<LinkedPageInfo> GetRelations(IEnumerable<IRelation> relations, bool linkChild)
    {
        foreach (var relation in relations.Where(x => !_ignoredTypeIds.Contains(x.RelationTypeId)))
        {
            if (relationTypeId == 0 || relation.RelationType.Id == this.relationTypeId)
            {
                var nodeId = linkChild ? relation.ChildId : relation.ParentId;
                var node = _entityService.Get(nodeId);
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

    private string GetContentPath(IEntitySlim node)
    {
        if (node == null) return string.Empty;

        var path = string.Empty;
        if (node.ParentId > -1)
        {
            var parent = _entityService.GetParent(node.Id);
            if (parent != null)
                path += GetContentPath(parent);
        }


        if (!string.IsNullOrWhiteSpace(path))
            return path + " > " + node.Name;

        return node.Name;
    }
}
