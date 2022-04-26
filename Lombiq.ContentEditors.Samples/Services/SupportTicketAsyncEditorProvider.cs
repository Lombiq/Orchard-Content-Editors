using Lombiq.ContentEditors.Models;
using Lombiq.ContentEditors.Samples.Constants;
using Lombiq.ContentEditors.Services;
using OrchardCore.ContentManagement;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Lombiq.ContentEditors.Samples.Constants.EditorGroups.SupportTicket;

namespace Lombiq.ContentEditors.Samples.Services;

public class SupportTicketAsyncEditorProvider : ContentItemAsyncEditorProviderBase<SupportTicketAsyncEditorProvider>
{
    public SupportTicketAsyncEditorProvider(IContentItemAsyncEditorProviderServices<SupportTicketAsyncEditorProvider> providerServices)
        : base(providerServices)
    {
    }

    protected override bool CanHandleContentType(string contentType) => contentType == ContentTypes.SupportTicket;

    public override Task<IEnumerable<AsyncEditorGroupDescriptor<ContentItem>>> DescribeEditorGroupsAsync() =>
        Task.FromResult(new[]
        {
            DescribeEditorGroup(Reporter, T["Reporter"]),
            DescribeEditorGroup(Details, T["Details"], isPublishGroup: true),
        }.AsEnumerable());
}
