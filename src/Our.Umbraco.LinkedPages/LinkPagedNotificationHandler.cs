//
// Notification handlers - NETCORE only - handle server variables and the menu
//
//
#if NETCOREAPP

using System;
using System.Collections.Generic;
using System.Linq;

using Microsoft.AspNetCore.Routing;

using Our.Umbraco.LinkedPages.Controllers;

using Umbraco.Cms.Core;
using Umbraco.Cms.Core.Events;
using Umbraco.Cms.Core.Models.Trees;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Cms.Core.Security;
using Umbraco.Cms.Core.Services;
using Umbraco.Extensions;

namespace Our.Umbraco.LinkedPages
{
    public class LinkPagedNotificationHandler :
        INotificationHandler<ServerVariablesParsingNotification>,
        INotificationHandler<MenuRenderingNotification>
    {
        private readonly LinkedPagesConfig _config;
        private readonly LinkGenerator _linkGenerator;
        private readonly IEntityService _entityService;
        private readonly IUserService _userService;
        private readonly IBackOfficeSecurityAccessor _backOfficeSecurityAccessor;

        public LinkPagedNotificationHandler(
            LinkedPagesConfig config,
            LinkGenerator linkGenerator,
            IEntityService entityService,
            IUserService userService,
            IBackOfficeSecurityAccessor backOfficeSercurityAccessor)
        {
            _config = config;
            _linkGenerator = linkGenerator;
            _entityService = entityService;
            _userService = userService;
            _backOfficeSecurityAccessor = backOfficeSercurityAccessor;
        }

        public void Handle(ServerVariablesParsingNotification notification)
        {
            notification.ServerVariables.Add(LinkedPages.Variables.Name, new Dictionary<string, object>
            {
                { LinkedPages.Variables.ApiRoute, _linkGenerator.GetUmbracoApiServiceBaseUrl<LinkedPagesApiController>(c => c.GetApi()) },
                { LinkedPages.Variables.ShowRelationType, _config.ShowType },
                { LinkedPages.Variables.RelationTypeAlias, _config.RelationType },
                { LinkedPages.Variables.IgnoredTypes, _config.ignoredTypes }
            });
        }

        public void Handle(MenuRenderingNotification notification)
        {
            if (notification.TreeAlias != Constants.Trees.Content) return;

            var currentUser = _backOfficeSecurityAccessor.BackOfficeSecurity.CurrentUser;
            var showMenu = currentUser.Groups.Any(x => x.Alias.InvariantContains("admin"));

            if (!showMenu && int.TryParse(notification.NodeId, out int nodeId))
            {
                var permissions = _userService.GetPermissions(currentUser, nodeId);
                showMenu = permissions.Any(x => x.AssignedPermissions.Contains(LinkedPages.ActionLetter));
            }

            if (showMenu)
            {

                var item = new MenuItem("linkedPages", "Linked Page")
                {
                    Icon = "link",
                    SeparatorBefore = true
                };

                item.AdditionalData.Add("actionView", LinkedPages.ActionView);

                notification.Menu.Items.Insert(notification.Menu.Items.Count - 1, item);
            }
        }
    }
}

#endif