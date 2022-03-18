using Lombiq.ContentEditors.Extensions;
using Lombiq.ContentEditors.Samples.Constants;
using Lombiq.ContentEditors.Samples.Drivers;
using Lombiq.ContentEditors.Samples.Migrations;
using Lombiq.ContentEditors.Samples.Models;
using Lombiq.ContentEditors.Samples.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.Modules;
using OrchardCore.ResourceManagement;

namespace Lombiq.ContentEditors.Samples
{
    [Feature(FeatureIds.Samples)]
    public class Startup : StartupBase
    {
        public override void ConfigureServices(IServiceCollection services)
        {
            services
                .AddContentPart<EmployeePart>()
                .WithMigration<EmployeeMigrations>()
                .WithAsyncEditor<EmployeeAsyncEditorProvider>();

            services
                .AddContentPart<SupportTicketPart>()
                .UseDisplayDriver<SupportTicketPartDisplayDriver>()
                .WithMigration<SupportTicketMigrations>()
                .WithAsyncEditor<SupportTicketAsyncEditorProvider>();

            services.AddTransient<IConfigureOptions<ResourceManagementOptions>, ResourceManagementOptionsConfiguration>();
        }
    }
}
