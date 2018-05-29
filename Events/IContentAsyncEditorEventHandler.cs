using Orchard.ContentManagement;
using Orchard.Events;

namespace Lombiq.ContentEditors.Events
{
    public interface IContentAsyncEditorEventHandler : IEventHandler
    {
        void BeforeUpdated(IContent content, string group, bool isNew);
        void Saved(IContent content, string group, bool isNew);
        void Published(IContent content, string group);
    }
}