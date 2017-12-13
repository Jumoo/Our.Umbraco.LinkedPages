using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using umbraco.interfaces;

namespace Our.Umbraco.LinkedPages
{
    // can't do this - because the attribute class is internal :( 
    // [ActionMetadata(Constants.Conventions.PermissionCategories.ContentCategory)]
    public class LinkAction : IAction
    {
        public char Letter => 'l';

        public bool ShowInNotifier => true;

        public bool CanBePermissionAssigned => true;

        public string Icon => "link";

        public string Alias => "linkPages";

        public string JsFunctionName => string.Empty;

        public string JsSource => string.Empty;
    }
}
