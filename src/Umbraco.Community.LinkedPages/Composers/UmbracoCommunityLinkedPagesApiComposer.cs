using Asp.Versioning;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi;
using Umbraco.Cms.Api.Common.OpenApi;
using Umbraco.Cms.Api.Management.OpenApi;
using Umbraco.Cms.Core.Composing;

namespace Umbraco.Community.LinkedPages.Composers
{
    public class UmbracoCommunityLinkedPagesApiComposer : IComposer
    {
        public void Compose(IUmbracoBuilder builder)
        {
            // settings first (used in services)
            builder.Services.Configure<LinkedPagesConfig>
                (builder.Config.GetSection("LinkedPages"));

            builder.AddLinkedPagesOpenApi();
        }
    }

    public static class LinkedPagesOpenApiExtensions
    {
        public static IUmbracoBuilder AddLinkedPagesOpenApi(this IUmbracoBuilder builder)
            => builder.AddBackOfficeOpenApiDocument(
                Constants.ApiName,
                document => document
                    .WithTitle("Umbraco Community Linked Pages Backoffice API")
                    .WithBackOfficeAuthentication()
                    .ConfigureOpenApiOptions(options =>
                    {
                        options.AddDocumentTransformer((doc, _, _) =>
                        {
                            doc.Info.Version = "Latest";
                            return Task.CompletedTask;
                        });
                    })
                );
    }
}
