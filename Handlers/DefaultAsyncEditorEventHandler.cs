using Lombiq.ContentEditors.Events;
using Lombiq.ContentEditors.Models;
using Orchard.ContentManagement;

namespace Lombiq.ContentEditors.Handlers
{
    public class DefaultAsyncEditorEventHandler : AsyncEditorEventHandlerBase
    {
        public override void Displaying(IContent content, string group)
        {
            var asnycEditorPart = content.AsAsyncEditorPart();

            if (asnycEditorPart == null) return;

            asnycEditorPart.IsAsyncEditorContext = true;

            if (!string.IsNullOrEmpty(group))
            {
                asnycEditorPart.LastDisplayedEditorGroupName = group;
                asnycEditorPart.SetCurrentEditorGroup(group);
            }
        }

        public override void Saved(IContent content, string group, bool isNew, bool published)
        {
            var asnycEditorPart = content.AsAsyncEditorPart();

            if (asnycEditorPart == null || string.IsNullOrEmpty(group)) return;

            asnycEditorPart.StoreCompletedEditorGroup(group);
            asnycEditorPart.LastUpdatedEditorGroupName = group;
        }
    }
}