using Umbraco.Core;
using Umbraco.Web.Actions;

namespace Our.Umbraco.LinkedPages
{
    public class LinkAction : IAction
    {
        public char Letter => LinkedPages.ActionLetter[0];

        public bool ShowInNotifier => true;

        public bool CanBePermissionAssigned => true;

        public string Icon => "link";

        public string Alias => "linkPages";

        public string Category => "structure";
    }
}
