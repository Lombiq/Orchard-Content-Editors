using Lombiq.ContentEditors.Demo.Constants;
using Lombiq.ContentEditors.Demo.Models;
using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentFields.Settings;
using OrchardCore.ContentManagement.Metadata;
using OrchardCore.ContentManagement.Metadata.Settings;
using OrchardCore.Data.Migration;

namespace Lombiq.ContentEditors.Demo.Migrations
{
    public class DemoMigrations : DataMigration
    {
        private readonly IContentDefinitionManager _contentDefinitionManager;

        public DemoMigrations(IContentDefinitionManager contentDefinitionManager) =>
            _contentDefinitionManager = contentDefinitionManager;

        public int Create()
        {
            _contentDefinitionManager.AlterPartDefinition(ContentTypes.DemoCustomer, part => part
                .WithField(nameof(DemoCustomer.FirstName), field => field
                    .OfType(nameof(TextField))
                    .WithDisplayName("First Name")
                    .WithSettings(new TextFieldSettings
                    {
                        Required = true,
                    }))
                .WithField(nameof(DemoCustomer.LastName), field => field
                    .OfType(nameof(TextField))
                    .WithDisplayName("Last Name")
                    .WithSettings(new TextFieldSettings
                    {
                        Required = true,
                    }))
                .WithField(nameof(DemoCustomer.DateOfBirth), field => field
                    .OfType(nameof(DateField))
                    .WithDisplayName("Date of Birth")
                    .WithSettings(new DateFieldSettings
                    {
                        Required = true,
                    }))
                .WithField(nameof(DemoCustomer.AdditionalNotes), field => field
                    .OfType(nameof(TextField))
                    .WithDisplayName("Additional Notes")
                    .WithEditor("TextArea")));

            _contentDefinitionManager.AlterTypeDefinition(ContentTypes.DemoCustomer, type => type
                .Creatable()
                .Securable()
                .Listable()
                .DisplayedAs("Demo Customer")
                .WithPart(ContentTypes.DemoCustomer));

            return 1;
        }
    }
}
