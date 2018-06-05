using Orchard.ContentManagement.Handlers;
using Orchard.Logging;
using System.Collections.Generic;

namespace Orchard.ContentManagement
{
    public static class ContentManagerExtensions
    {
        public static IContent NewClone(
            this IContentManager contentManager, 
            ContentItem contentItem, 
            IEnumerable<IContentHandler> handlers,
            ILogger logger)
        {
            var cloneContentItem = contentManager.New(contentItem.ContentType);
            var context = new CloneContentContext(contentItem, cloneContentItem);

            handlers.Invoke(handler => handler.Cloning(context), logger);
            handlers.Invoke(handler => handler.Cloned(context), logger);

            return cloneContentItem;
        }
    }
}