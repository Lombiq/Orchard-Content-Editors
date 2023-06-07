using Lombiq.ContentEditors.Samples.Models;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;
using static Lombiq.ContentEditors.Samples.Constants.ContentTypes;

namespace Lombiq.ContentEditors.Samples.Migrations;

public class SupportTicketMigrations : DataMigration
{
    private readonly IContentDefinitionManager _contentDefinitionManager;

    public SupportTicketMigrations(IContentDefinitionManager contentDefinitionManager) =>
        _contentDefinitionManager = contentDefinitionManager;

    public int Create()
    {
        _contentDefinitionManager.AlterTypeDefinition(SupportTicket, type => type
            .Listable()
            .WithPart(nameof(SupportTicketPart)));

        return 1;
    }
}
