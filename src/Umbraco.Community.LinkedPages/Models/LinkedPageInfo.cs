using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Umbraco.Community.LinkedPages.Models;

[JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
public class LinkedPageInfo
{
    public int RelationId { get; set; }
    public int PageId { get; set; }
    public string Name { get; set; }
    public string Path { get; set; }
    public int RelationTypeId { get; set; }
    public string RelationType { get; set; }
}
