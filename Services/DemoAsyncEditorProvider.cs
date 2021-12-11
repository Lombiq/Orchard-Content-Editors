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
                new AsyncEditorGroup
                {
                    Name = "Group1", DisplayText = T["Group 1"], IsAccessible = true,
                },
                new AsyncEditorGroup
                {
                    Name = "Group2", DisplayText = T["Group 2"], IsAccessible = true,
                },
                new AsyncEditorGroup
                {
                    Name = "Group3", DisplayText = T["Group 3"], IsAccessible = false, IsPublishGroup = true,
                },
            }.AsEnumerable());
    }
}
