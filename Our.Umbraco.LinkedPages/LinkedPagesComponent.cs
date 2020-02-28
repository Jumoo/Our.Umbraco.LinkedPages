using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

using Our.Umbraco.LinkedPages.Controllers;

using Umbraco.Core;
using Umbraco.Core.Composing;
using Umbraco.Web;
using Umbraco.Web.JavaScript;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Trees;


namespace Our.Umbraco.LinkedPages
{
    public class LinkedPagesComposer 
        : ComponentComposer<LinkedPagesComponent>
    { }

    public class LinkedPagesComponent : IComponent
    {
        public void Initialize()
        {
            ContentTreeController.MenuRendering += ContentTreeController_MenuRendering;
            ServerVariablesParser.Parsing += ServerVariablesParser_Parsing;
        }

        private void ServerVariablesParser_Parsing(object sender, Dictionary<string, object> e)
        {
            if (HttpContext.Current == null)
                throw new InvalidOperationException("This method requires an HttpContext");

            var typeAlias = ConfigurationManager.AppSettings["LinkedPages.RelationType"];
            var showType = ConfigurationManager.AppSettings["LinkedPages.ShowType"]
                .InvariantEquals("true");

            var urlHelper = new UrlHelper(new RequestContext(
                new HttpContextWrapper(HttpContext.Current), new RouteData()));

            e.Add("LinkedPages", new Dictionary<string, object>
            {
                { "LinkedPageApi", urlHelper.GetUmbracoApiServiceBaseUrl<LinkedPagesApiController>( c => c.GetApi()) },
                { "showRelationType", showType },
                { "relationTypeAlias", typeAlias }
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

                linkedPagedItem.AdditionalData.Add("actionView",
                    UriUtility.ToAbsolute("/App_Plugins/LinkedPages/linkedDialog.html"));

                e.Menu.Items.Insert(e.Menu.Items.Count - 1, linkedPagedItem);
            }
        }

        public void Terminate()
        {
            // end.
        }
    }
}
