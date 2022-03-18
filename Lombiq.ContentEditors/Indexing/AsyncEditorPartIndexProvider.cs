using Lombiq.ContentEditors.Models;
using OrchardCore.ContentManagement;
using System.Linq;
using YesSql.Indexes;

namespace Lombiq.ContentEditors.Indexing
{
    public class AsyncEditorPartIndexProvider : IndexProvider<ContentItem>
    {
        public override void Describe(DescribeContext<ContentItem> context) =>
            context.For<AsyncEditorPartIndex>()
                .When(contentItem => contentItem.Has<AsyncEditorPart>())
                .Map(contentItem =>
                {
                    var asyncEditorPart = contentItem.As<AsyncEditorPart>();

                    return asyncEditorPart?.FilledEditorGroups.SelectMany(
                        filledEditorGroup => filledEditorGroup.Value,
                        (filledEditorGroup, editorGroup) => new AsyncEditorPartIndex
                        {
                            ContentItemId = asyncEditorPart.ContentItem.ContentItemId,
                            AsyncEditorId = filledEditorGroup.Key,
                            FilledEditorGroup = editorGroup,
                        }).ToList();
                });
    }
}
