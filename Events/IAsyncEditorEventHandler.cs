using Orchard.ContentManagement;
using Orchard.Events;

namespace Lombiq.ContentEditors.Events
{
    public interface IAsyncEditorEventHandler : IEventHandler
    {
        void Updating(IContent content, string group, bool isNew);
        void Updated(IContent content, string group, bool isNew, bool modelStateIsValid);
        void Saved(IContent content, string group, bool isNew, bool published);
    }


    public abstract class AsyncEditorEventHandlerBase : IAsyncEditorEventHandler
    {
        public virtual void Updating(IContent content, string group, bool isNew) { }
        public virtual void Updated(IContent content, string group, bool isNew, bool modelStateIsValid) { }
        public virtual void Saved(IContent content, string group, bool isNew, bool published) { }
    }
}