using Lombiq.ContentEditors.Indexing;
using Lombiq.HelpfulLibraries.OrchardCore.Data;
using OrchardCore.Data.Migration;
using YesSql.Sql;

namespace Lombiq.ContentEditors.Migrations;

public class AsyncEditorMigrations : DataMigration
{
    public int Create()
    {
        SchemaBuilder.CreateMapIndexTable<AsyncEditorPartIndex>(table => table
            .Column<string>(nameof(AsyncEditorPartIndex.ContentItemId), column => column.WithCommonUniqueIdLength())
            .Column<string>(nameof(AsyncEditorPartIndex.AsyncEditorId))
            .Column<string>(nameof(AsyncEditorPartIndex.FilledEditorGroup))
        );

        return 1;
    }
}
