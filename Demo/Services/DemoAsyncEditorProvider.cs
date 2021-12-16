using Lombiq.ContentEditors.Demo.Constants;
using Lombiq.ContentEditors.Models;
using Lombiq.ContentEditors.Services;
using OrchardCore.ContentManagement;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Lombiq.ContentEditors.Demo.Constants.EditorGroups.DemoCustomer;

namespace Lombiq.ContentEditors.Demo.Services
{
    public class DemoAsyncEditorProvider : ContentItemAsyncEditorProviderBase<DemoAsyncEditorProvider>
    {
        public DemoAsyncEditorProvider(IContentItemAsyncEditorProviderServices<DemoAsyncEditorProvider> providerServices)
            : base(providerServices)
        {
        }

        protected override bool CanHandleContentType(string contentType) => contentType == ContentTypes.DemoCustomer;

        public override Task<IEnumerable<AsyncEditorGroupDescriptor<ContentItem>>> DescribeEditorGroupsAsync() =>
            Task.FromResult(new[]
            {
                DescribeEditorGroup(PersonalDetails, T["Personal Details"]),
                DescribeEditorGroup(AdditionalNotes, T["Additional Notes"], true),
            }.AsEnumerable());
    }
}
