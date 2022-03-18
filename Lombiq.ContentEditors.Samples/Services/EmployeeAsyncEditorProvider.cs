using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Lombiq.ContentEditors.Models;
using Lombiq.ContentEditors.Samples.Constants;
using Lombiq.ContentEditors.Services;
using OrchardCore.ContentManagement;
using static Lombiq.ContentEditors.Samples.Constants.EditorGroups.Employee;

namespace Lombiq.ContentEditors.Samples.Services
{
    public class EmployeeAsyncEditorProvider : ContentItemAsyncEditorProviderBase<EmployeeAsyncEditorProvider>
    {
        public EmployeeAsyncEditorProvider(IContentItemAsyncEditorProviderServices<EmployeeAsyncEditorProvider> providerServices)
            : base(providerServices)
        {
        }

        protected override bool CanHandleContentType(string contentType) => contentType == ContentTypes.Employee;

        public override Task<IEnumerable<AsyncEditorGroupDescriptor<ContentItem>>> DescribeEditorGroupsAsync() =>
            Task.FromResult(new[]
            {
                DescribeEditorGroup(PersonalDetails, T["Personal Details"]),
                DescribeEditorGroup(EmploymentDetails, T["Employment Details"], isPublishGroup: true),
            }.AsEnumerable());
    }
}
