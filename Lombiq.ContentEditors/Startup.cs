using Lombiq.ContentEditors.Constants;
using Lombiq.ContentEditors.Indexing;
using Lombiq.ContentEditors.Migrations;
using Lombiq.ContentEditors.Models;
using Lombiq.ContentEditors.Services;
using Lombiq.HelpfulLibraries.Common.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrchardCore.ContentManagement;
using OrchardCore.Data;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;
using OrchardCore.ResourceManagement;

namespace Lombiq.ContentEditors;

[Feature(FeatureIds.AsyncEditor)]
public class Startup : StartupBase
{
    public override void ConfigureServices(IServiceCollection services)
    {
        services.AddLazyInjectionSupport();
        services.AddTransient<IConfigureOptions<ResourceManagementOptions>, ResourceManagementOptionsConfiguration>();
        services.AddScoped(typeof(IContentItemAsyncEditorProviderServices<>), typeof(ContentItemAsyncEditorProviderServices<>));
        services.AddContentPart<AsyncEditorPart>();
        services.AddScoped<IDataMigration, AsyncEditorMigrations>();
        services.AddIndexProvider<AsyncEditorPartIndexProvider>();
        services.AddScoped<IAsyncEditorProvider<ContentItem>, DefaultContentItemAsyncEditorProvider>();
    }
}
