using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.ModelBinding;
using System;

namespace Lombiq.ContentEditors.Services
{
    public class ContentItemAsyncEditorProviderServices<T> : IContentItemAsyncEditorProviderServices<T>
        where T : IAsyncEditorProvider<ContentItem>
    {
        public Lazy<IContentManager> ContentManager { get; }
        public Lazy<IContentItemDisplayManager> ContentItemDisplayManager { get; }
        public Lazy<IDisplayHelper> DisplayHelper { get; }
        public Lazy<IShapeFactory> ShapeFactory { get; }
        public Lazy<IUpdateModelAccessor> UpdateModelAccessor { get; }
        public Lazy<IStringLocalizer<T>> StringLocalizer { get; }

        public ContentItemAsyncEditorProviderServices(
            Lazy<IContentManager> contentManager,
            Lazy<IContentItemDisplayManager> contentItemDisplayManager,
            Lazy<IDisplayHelper> displayHelper,
            Lazy<IShapeFactory> shapeFactory,
            Lazy<IUpdateModelAccessor> updateModelAccessor,
            Lazy<IStringLocalizer<T>> stringLocalizer)
        {
            ContentManager = contentManager;
            ContentItemDisplayManager = contentItemDisplayManager;
            DisplayHelper = displayHelper;
            ShapeFactory = shapeFactory;
            UpdateModelAccessor = updateModelAccessor;
            StringLocalizer = stringLocalizer;
        }
    }
}
