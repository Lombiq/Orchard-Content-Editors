using Orchard.ContentManagement;
using Orchard.Events;

namespace Lombiq.ContentEditors.Events
{
    public interface IContentAsyncEditorEventHandler : IEventHandler
    {
        void Updating(IContent content, string group, bool isNew);
        void Saved(IContent content, string group, bool isNew, bool published);
    }
}