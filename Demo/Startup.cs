using Lombiq.ContentEditors.Constants;
using Lombiq.ContentEditors.Demo.Migrations;
using Lombiq.ContentEditors.Demo.Models;
using Lombiq.ContentEditors.Demo.Services;
using Lombiq.ContentEditors.Services;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.ContentManagement;
using OrchardCore.Data.Migration;
using OrchardCore.Modules;

namespace Lombiq.ContentEditors.Demo
{
    [Feature(FeatureIds.Demo)]
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services.AddContentPart<DemoCustomer>();
            services.AddScoped<IDataMigration, DemoMigrations>();
            services.AddScoped<IAsyncEditorProvider<ContentItem>, DemoAsyncEditorProvider>();
        }
    }
}
