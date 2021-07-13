
#if NETFRAMEWORK
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
#endif

namespace Our.Umbraco.LinkedPages
{
#if NETFRAMEWORK
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy))]
#endif
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
