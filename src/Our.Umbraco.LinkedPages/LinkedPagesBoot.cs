#if NETCOREAPP
using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Notifications;
using Umbraco.Extensions;
#else
using Umbraco.Core;
using Umbraco.Core.Composing;
#endif

namespace Our.Umbraco.LinkedPages
{
    public class LinkedPagesBoot : IComposer
    {
#if NETCOREAPP
        public void Compose(IUmbracoBuilder builder)
        {
            builder.Services.AddUnique<LinkedPagesConfig>();

            builder.AddNotificationHandler<ServerVariablesParsingNotification, LinkPagedNotificationHandler>();
            builder.AddNotificationHandler<MenuRenderingNotification, LinkPagedNotificationHandler>();

        }
#else
        public void Compose(Composition composition)
        {
            composition.RegisterUnique<LinkedPagesConfig>();

            composition.Components().Append<LinkedPagesComponent>();
        }
#endif
    }

}
