using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.ModelBinding;
using System;

namespace Lombiq.ContentEditors.Services
{
    public interface IContentItemAsyncEditorProviderServices<T>
        where T : IAsyncEditorProvider<ContentItem>
    {
        Lazy<IContentManager> ContentManager { get; }
        Lazy<IContentItemDisplayManager> ContentItemDisplayManager { get; }
        Lazy<IDisplayHelper> DisplayHelper { get; }
        Lazy<IShapeFactory> ShapeFactory { get; }
        Lazy<IUpdateModelAccessor> UpdateModelAccessor { get; }
        Lazy<IStringLocalizer<T>> StringLocalizer { get; }
    }
}
