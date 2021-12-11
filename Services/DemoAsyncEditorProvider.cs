using Lombiq.ContentEditors.Extensions;
using Lombiq.ContentEditors.Models;
using OrchardCore.ContentManagement;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.ContentEditors.Services
{
    public class DemoAsyncEditorProvider : ContentItemAsyncEditorProviderBase<DemoAsyncEditorProvider>
    {
        public DemoAsyncEditorProvider(IContentItemAsyncEditorProviderServices<DemoAsyncEditorProvider> providerServices)
            : base(providerServices)
        {
        }

        public override Task<IEnumerable<AsyncEditorGroup>> GetOrderedEditorGroupsAsync(AsyncEditorContext<ContentItem> context) =>
            Task.FromResult(new[]
            {
                CreateEditorGroup(context, "Group1", T["Group 1"]),
                CreateEditorGroup(context, "Group2", T["Group 2"]),
                CreateEditorGroup(context, "Group3", T["Group 3"], context.Content.HasFilledEditorGroup(context.AsyncEditorId, "Group2")),
            }.AsEnumerable());
    }
}
