using Lombiq.ContentEditors.Samples.Models;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;
using System.Threading.Tasks;
using static Lombiq.ContentEditors.Samples.Constants.ContentTypes;

namespace Lombiq.ContentEditors.Samples.Migrations;

// This is the migration class for the Support Ticket content type. Nothing specific to async editors here.
public class SupportTicketMigrations(IContentDefinitionManager contentDefinitionManager) : DataMigration
{
    public async Task<int> CreateAsync()
    {
        await contentDefinitionManager.AlterTypeDefinitionAsync(SupportTicket, type => type
            .Listable()
            .WithPart(nameof(SupportTicketPart)));

        return 1;
    }
}

// NEXT STATION: Drivers/SupportTicketPartDisplayDriver.cs
