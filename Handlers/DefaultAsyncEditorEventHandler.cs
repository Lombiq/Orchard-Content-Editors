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

            asnycEditorPart.LastDisplayedEditorGroupName = group;
            asnycEditorPart.SetCurrentEditorGroup(group);
            asnycEditorPart.IsAsyncEditorContext = true;
        }

        public override void Saved(IContent content, string group, bool isNew, bool published)
        {
            var asnycEditorPart = content.AsAsyncEditorPart();

            if (asnycEditorPart == null) return;

            asnycEditorPart.StoreCompletedEditorGroup(group);
            asnycEditorPart.LastUpdatedEditorGroupName = group;
        }
    }
}