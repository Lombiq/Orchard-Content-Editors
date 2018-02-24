using Lombiq.EditorGroups.Constants;
using Lombiq.EditorGroups.Models;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;
using Orchard.Fields.Settings;
using Piedone.HelpfulExtensions;

namespace Lombiq.EditorGroups.Migrations
{
    public class AsyncEditorTestMigrations : DataMigrationImpl
    {
        public int Create()
        {
            ContentDefinitionManager.AlterPartDefinition(nameof(TestContentPart), part => part
                .Attachable(false)
                .WithInputField(AsyncEditorTestConstants.TestField1, field => field
                    .WithInputFieldSettings(new InputFieldSettings { Required = true }))
                .WithInputField(AsyncEditorTestConstants.TestField2, field => field
                    .WithInputFieldSettings(new InputFieldSettings())));

            ContentDefinitionManager.AlterTypeDefinition(AsyncEditorTestConstants.TestContentTypeWithoutGroups,
                cfg => cfg
                    .Creatable()
                    .Listable()
                    .Draftable()
                    .WithPart("TitlePart")
                    .WithPart("CommonPart")
                    .WithPart("BodyPart")
                    .WithPart(nameof(TestContentPart))
                    .WithPart(nameof(AsyncEditorPart))
                );

            ContentDefinitionManager.AlterTypeDefinition(AsyncEditorTestConstants.TestContentTypeWithGroups,
                cfg => cfg
                    .Creatable()
                    .Listable()
                    .Draftable()
                    .WithPart("TitlePart")
                    .WithPart("CommonPart")
                    .WithPart("BodyPart")
                    .WithPart(nameof(TestContentPart))
                    .WithPart(nameof(AsyncEditorPart))
                );



            return 1;
        }
    }
}