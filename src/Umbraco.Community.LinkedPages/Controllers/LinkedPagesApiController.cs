using Asp.Versioning;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Umbraco.Cms.Core.Models;
using Umbraco.Cms.Core.Services;
using Umbraco.Community.LinkedPages.Models;
using Umbraco.Cms.Core;
using Umbraco.Extensions;
using Umbraco.Cms.Core.Models.Entities;
using Umbraco.Cms.Api.Common.Builders;

namespace Umbraco.Community.LinkedPages.Controllers;

[ApiVersion("1.0")]
[ApiExplorerSettings(GroupName = "Umbraco.Community.LinkedPages")]
public class LinkedPagesApiController : LinkedPagesApiControllerBase
{
    private readonly IRelationService _relationService;
    private readonly IEntityService _entityService;
    private readonly LinkedPagesConfig _config;
    private readonly IIdKeyMap _idKeyMap;

    private string defaultRelationType = Cms.Core.Constants.Conventions.RelationTypes.RelateDocumentOnCopyAlias;
    private int relationTypeId = 0;
    private int[] _ignoredTypeIds;

    public LinkedPagesApiController(
    IRelationService relationService,
    IEntityService entityService,
    IOptions<LinkedPagesConfig> config,
    IIdKeyMap idKeyMap)
    {
        _relationService = relationService;
        _entityService = entityService;
        _config = config.Value;

        _ignoredTypeIds = GetIgnoredTypeIds();
        _idKeyMap = idKeyMap;
    }

    private int[] GetIgnoredTypeIds()
    {
        var ignore = _config.IgnoredTypes.ToDelimitedList();
        var types = _relationService.GetAllRelationTypes();

        return types.Where(x => ignore.InvariantContains(x.Alias))
            .Select(x => x.Id)
            .ToArray();
    }

    [HttpGet("IgnoredTypeAlias")]
    [ProducesResponseType<List<string>>(StatusCodes.Status200OK)]
    public List<string> getIgnoredTypeAlias()
    {
        var typeIds = GetIgnoredTypeIds();
        List<string> ignoredTypes = [];
        foreach (var typeId in typeIds)
        {
            var ignoredType = _relationService.GetRelationTypeById(typeId);
            if (ignoredType == null) continue;
            ignoredTypes.Add(ignoredType.Alias);
        }

        return ignoredTypes;
    }

    [HttpGet("ChildLinks")]
    [ProducesResponseType<IEnumerable<LinkedPageInfo>>(StatusCodes.Status200OK)]
    public IEnumerable<LinkedPageInfo> GetChildLinks(Guid key)
    {
        var idAttempt = _idKeyMap.GetIdForKey(key, UmbracoObjectTypes.Document);
        if (!idAttempt.Success) return Enumerable.Empty<LinkedPageInfo>();
        var relations = _relationService.GetByParentId(idAttempt.Result);
        if (!relations.Any())
            return Enumerable.Empty<LinkedPageInfo>();

        return GetRelations(relations, true);
    }

    [HttpGet("ParentLinks")]
    [ProducesResponseType<IEnumerable<LinkedPageInfo>>(StatusCodes.Status200OK)]
    public IEnumerable<LinkedPageInfo> GetParentLinks(Guid key)
    {
        var idAttempt = _idKeyMap.GetIdForKey(key, UmbracoObjectTypes.Document);
        if (!idAttempt.Success) return Enumerable.Empty<LinkedPageInfo>();
        var relations = _relationService.GetByChildId(idAttempt.Result);
        if (!relations.Any())
            return Enumerable.Empty<LinkedPageInfo>();

        return GetRelations(relations, false);
    }

    [HttpPost("CreateLink")]
    [ProducesResponseType<IEnumerable<LinkedPageInfo>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public IActionResult CreateLink(Guid parent, Guid child)
    {
        var parentNode = _entityService.Get(parent);
        var childNode = _entityService.Get(child);

        if (parentNode == null || childNode == null)
            throw new KeyNotFoundException();
        var typeAlias = string.IsNullOrWhiteSpace(_config.RelationType) ?
            defaultRelationType : _config.RelationType;
        var relationType = _relationService.GetRelationTypeByAlias(typeAlias);
        if (relationType == null)
            return BadRequest(new ProblemDetailsBuilder()
                .WithTitle($"Cannot create relation of type {typeAlias}")
                .WithDetail("Relation type was null, type alias invalid.")
                .Build());

        var relation = new Relation(parentNode.Id, childNode.Id, relationType);
        _relationService.Save(relation);

        return Ok(GetChildLinks(parent));
    }

    [HttpDelete("RemoveLink")]
    [ProducesResponseType<IEnumerable<LinkedPageInfo>>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public IActionResult RemoveLink(int key, Guid currentPage)
    {
        var relation = _relationService.GetById(key);
        if (relation == null)
            return BadRequest(new ProblemDetailsBuilder()
                .WithTitle("Failed to remove, child Link not found.")
                .WithDetail("Relation was null, could not remove link.")
                .Build());

        _relationService.Delete(relation);
        return Ok(GetChildLinks(currentPage));
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
                    RelationTypeId = relation.RelationTypeId,
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
        return path + "/" + node.Name;
    }
}
