using Lombiq.ContentEditors.Models;
using OrchardCore.ContentManagement;
using System.Collections.Generic;
using System.Linq;

namespace Lombiq.ContentEditors.Extensions;

public static class ContentExtensions
{
    public static void SetFilledEditorGroup(this IContent content, string asyncEditorId, string editorGroup)
    {
        var part = content.GetOrWeldAsyncEditorPart();
        part.FilledEditorGroups.AddToList(asyncEditorId, editorGroup);
        content.Apply(part);
    }

    public static IEnumerable<string> GetFilledEditorGroups(this IContent content, string asyncEditorId) =>
        content
            .GetOrWeldAsyncEditorPart()
            .FilledEditorGroups
            .GetMaybe(asyncEditorId) ?? Enumerable.Empty<string>();

    public static bool IsEditorGroupFilled(this IContent content, string asyncEditorId, string editorGroup) =>
        content.GetFilledEditorGroups(asyncEditorId).Contains(editorGroup);

    public static AsyncEditorPart GetOrWeldAsyncEditorPart(this IContent content)
    {
        if (!content.ContentItem.Has<AsyncEditorPart>())
        {
            content.Weld(new AsyncEditorPart());
        }

        return content.As<AsyncEditorPart>();
    }
}
