namespace Our.Umbraco.LinkedPages;

public class LinkedPages
{
    public const string Alias = "linkPages";
    public const string Category = "structure";

    public const string ProductName = "Our.Umbraco.LinkedPages";
    public const string ActionLetter = "l";

    public const string ActionView = "/App_Plugins/LinkedPages/linkedDialog.html";

    public static class Variables
    {
        public const string Name = "LinkedPages";

        public const string ApiRoute = "LinkedPageApi";
        public const string ShowRelationType = "showRelationType";
        public const string RelationTypeAlias = "relationTypeAlias";
        public const string IgnoredTypes = "ignoredTypes";
    }
}
