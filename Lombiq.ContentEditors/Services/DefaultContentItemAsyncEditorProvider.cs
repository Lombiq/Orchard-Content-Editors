using Lombiq.ContentEditors.Models;
using OrchardCore.ContentManagement;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.ContentEditors.Services;

public class DefaultContentItemAsyncEditorProvider(
    IContentItemAsyncEditorProviderServices<DefaultContentItemAsyncEditorProvider> providerServices)
    : ContentItemAsyncEditorProviderBase<DefaultContentItemAsyncEditorProvider>(providerServices)
{
    public override string Name => "Default";

    public override Task<IEnumerable<AsyncEditorGroupDescriptor<ContentItem>>> DescribeEditorGroupsAsync() =>
        Task.FromResult(new[]
        {
            DescribeEditorGroup(name: null, T["Content"], isPublishGroup: true),
        }.AsEnumerable());
}
