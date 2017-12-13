using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

using Our.Umbraco.LinkedPages.Controllers;
using Umbraco.Core;
using Umbraco.Core.Services;
using Umbraco.Web;
using Umbraco.Web.Models.Trees;
using Umbraco.Web.Trees;
using Umbraco.Web.UI.JavaScript;

namespace Our.Umbraco.LinkedPages
{
    public class LinkedPagesApplicationEventHandler : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            ContentTreeController.MenuRendering += ContentTreeController_MenuRendering;
            ServerVariablesParser.Parsing += ServerVariablesParser_Parsing;
        }

        private void ServerVariablesParser_Parsing(object sender, Dictionary<string, object> e)
        {
            if (HttpContext.Current != null)
            {
                var urlHelper = new UrlHelper(HttpContext.Current.Request.RequestContext);
                e.Add("LinkedPages", new Dictionary<string, object>
                {
                    { "LinkedPageApi", urlHelper.GetUmbracoApiServiceBaseUrl<LinkedPagesApiController>(
                       controller => controller.GetApiController() ) }
                });

            }
        }

        private void ContentTreeController_MenuRendering(TreeControllerBase sender, MenuRenderingEventArgs e)
        {
            if (sender.TreeAlias.InvariantEquals("content"))
            {
                if (int.TryParse(e.NodeId, out int nodeId))
                {
                    var permissions = sender.Services.UserService.GetPermissions(sender.Security.CurrentUser, nodeId);
                    var letter = "L";
                    if (permissions.Any(x => x.AssignedPermissions.InvariantContains(letter)))
                    {
                        var linkedItemsItem = new MenuItem("linkedPages", "Linked Pages")
                        {
                            Icon = "link",
                            SeperatorBefore = true
                        };

                        linkedItemsItem.AdditionalData.Add("actionView", "/App_Plugins/LinkedPages/linkedDialog.html");

                        e.Menu.Items.Insert(e.Menu.Items.Count - 1, linkedItemsItem);
                    }
                }
            }
        }
    }
}
