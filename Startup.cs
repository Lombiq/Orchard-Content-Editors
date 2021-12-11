using Lombiq.ContentEditors.Constants;
using Lombiq.ContentEditors.Services;
using Lombiq.HelpfulLibraries.Libraries.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrchardCore.ContentManagement;
using OrchardCore.Modules;
using OrchardCore.ResourceManagement;

namespace Lombiq.ContentEditors
{
    [Feature(FeatureIds.ContentEditors)]
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddLazyInjectionSupport();
            services.AddTransient<IConfigureOptions<ResourceManagementOptions>, ResourceManagementOptionsConfiguration>();
            services.AddScoped<IAsyncEditorProvider<ContentItem>, DemoAsyncEditorProvider>();
            services.AddScoped(typeof(IContentItemAsyncEditorProviderServices<>), typeof(ContentItemAsyncEditorProviderServices<>));
        }
    }
}
