//
// Component is only used in netcore app to hook into menu and server variable parser. 
//
#if NETFRAMEWORK

using Our.Umbraco.LinkedPages.Controllers;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Routing;
using System.Web;
using System;

using Umbraco.Core.Composing;
using Umbraco.Web.JavaScript;
using Umbraco.Web.Trees;
using Umbraco.Core;
using System.Web.Mvc;
using Umbraco.Web;
using System.Linq;
using Umbraco.Web.Models.Trees;

namespace Our.Umbraco.LinkedPages
{
    public class LinkedPagesComponet : IComponent
    {
        private readonly LinkedPagesConfig _config;

        public LinkedPagesComponet(LinkedPagesConfig config)
        {
            _config = config;
        }

        public void Initialize()
        {
            ContentTreeController.MenuRendering += ContentTreeController_MenuRendering;
            ServerVariablesParser.Parsing += ServerVariablesParser_Parsing;
        }

        private void ServerVariablesParser_Parsing(object sender, System.Collections.Generic.Dictionary<string, object> e)
        {
            if (HttpContext.Current == null)
                throw new InvalidOperationException("This method requires an HttpContext");


            var urlHelper = new UrlHelper(new RequestContext(
                new HttpContextWrapper(HttpContext.Current), new RouteData()));

            e.Add(LinkedPages.Variables.Name, new Dictionary<string, object>
            {
                { LinkedPages.Variables.ApiRoute, urlHelper.GetUmbracoApiServiceBaseUrl<LinkedPagesApiController>( c => c.GetApi()) },
                { LinkedPages.Variables.ShowRelationType, _config.ShowType },
                { LinkedPages.Variables.RelationTypeAlias, _config.RelationType },
                { LinkedPages.Variables.IgnoredTypes, _config.ignoredTypes }
            });
        }

        private void ContentTreeController_MenuRendering(TreeControllerBase sender, MenuRenderingEventArgs e)
        {
            // only the content tree and not the root.
            if (!sender.TreeAlias.InvariantEquals("content") || e.NodeId == "-1") return;

            bool showMenu = sender.Security.CurrentUser.Groups.Any(x => x.Alias.InvariantContains("admin"));
            if (!showMenu && int.TryParse(e.NodeId, out int nodeId))
            {
                var permissions = sender.Services.UserService
                    .GetPermissions(sender.Security.CurrentUser, nodeId);

                showMenu = permissions.Any(x => x.AssignedPermissions.Contains(LinkedPages.ActionLetter));
            }

            if (showMenu)
            {
                var linkedPagedItem = new MenuItem("linkedPages", "Linked Pages")
                {
                    Icon = "link",
                    SeparatorBefore = true
                };

                linkedPagedItem.AdditionalData.Add("actionView", UriUtility.ToAbsolute(LinkedPages.ActionView));

                e.Menu.Items.Insert(e.Menu.Items.Count - 1, linkedPagedItem);
            }
        }

        public void Terminate()
        {
        }
    }

}
#endif