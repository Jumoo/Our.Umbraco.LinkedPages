using System.Collections.Generic;
using System.Linq;

using Microsoft.Extensions.DependencyInjection;

using Umbraco.Cms.Core.Composing;
using Umbraco.Cms.Core.DependencyInjection;
using Umbraco.Cms.Core.Manifest;
using Umbraco.Cms.Core.Notifications;

namespace Our.Umbraco.LinkedPages;

public class LinkedPagesBoot : IComposer
{
    public void Compose(IUmbracoBuilder builder)
    {
        builder.AddLinkedPages();
    }
}

public static class LinkedPagesBuilderExtensions
{
    public static IUmbracoBuilder AddLinkedPages(this IUmbracoBuilder builder)
    {
        if (builder.Services.Any(x => x.ServiceType == typeof(LinkedPagesConfig)))
            return builder;

        builder.Services.AddSingleton<LinkedPagesConfig>();
        builder.AddNotificationHandler<ServerVariablesParsingNotification, LinkPagedNotificationHandler>();
        builder.AddNotificationHandler<MenuRenderingNotification, LinkPagedNotificationHandler>();

        if (!builder.ManifestFilters().Has<LinkedPagesManifestFilter>())
            builder.ManifestFilters().Append<LinkedPagesManifestFilter>();

        return builder;
    }
}

internal class LinkedPagesManifestFilter : IManifestFilter
{
    public void Filter(List<PackageManifest> manifests)
    {
        manifests.Add(new PackageManifest
        {
            PackageName = LinkedPages.ProductName,
            Version = typeof(LinkedPages).Assembly.GetName().Version.ToString(3),
            AllowPackageTelemetry = true,
            Scripts = new[]
            {
                "/App_Plugins/LinkedPages/linkedPagesDialogController.js",
                "/App_Plugins/LinkedPages/linkedPagesService.js"
            },
            Stylesheets = new[]
            {
                "/App_Plugins/LinkedPages/linkedpages.css"
            }
        });
    }
}
