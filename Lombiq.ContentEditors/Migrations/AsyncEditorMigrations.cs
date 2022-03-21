using Lombiq.ContentEditors.Indexing;
using Lombiq.HelpfulLibraries.Libraries.Database;
using OrchardCore.Data.Migration;
using YesSql.Sql;

namespace Lombiq.ContentEditors.Migrations
{
    public class AsyncEditorMigrations : DataMigration
    {
        public int Create()
        {
            SchemaBuilder.CreateMapIndexTable<AsyncEditorPartIndex>(table => table
                .Column<string>(nameof(AsyncEditorPartIndex.ContentItemId), c => c.WithCommonUniqueIdLength())
                .Column<string>(nameof(AsyncEditorPartIndex.AsyncEditorId))
                .Column<string>(nameof(AsyncEditorPartIndex.FilledEditorGroup))
            );

            return 1;
        }
    }
}