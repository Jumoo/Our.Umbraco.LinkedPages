using Umbraco.Cms.Core.Actions;

namespace Our.Umbraco.LinkedPages;

public class LinikedAction : IAction
{
    public char Letter => LinkedPages.ActionLetter[0];

    public bool ShowInNotifier => true;

    public bool CanBePermissionAssigned => true;

    public string Icon => "link";

    public string Alias => LinkedPages.Alias;

    public string Category => LinkedPages.Category;
}
