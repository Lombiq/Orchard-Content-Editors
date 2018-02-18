using Lombiq.EditorGroups.Constants;
using Lombiq.EditorGroups.Models;
using Orchard.ContentManagement.MetaData;
using Orchard.Core.Contents.Extensions;
using Orchard.Data.Migration;

namespace Lombiq.EditorGroups.Migrations
{
    public class EditorGroupsTestMigrations : DataMigrationImpl
    {
        public int Create()
        {
            ContentDefinitionManager.AlterTypeDefinition(EditorGroupsTestConstants.TestContentTypeName,
                cfg => cfg
                    .Creatable()
                    .Listable()
                    .Draftable()
                    .WithPart("TitlePart")
                    .WithPart("CommonPart")
                    .WithPart("BodyPart")
                    .WithPart(nameof(EditorGroupsPart))
                );

            return 1;
        }
    }
}