using Lombiq.ContentEditors.Models;
using Lombiq.ContentEditors.Samples.Constants;
using Lombiq.ContentEditors.Services;
using OrchardCore.ContentManagement;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Lombiq.ContentEditors.Samples.Constants.EditorGroups.Employee;

namespace Lombiq.ContentEditors.Samples.Services;

// If you want to create an async editor to your content type then you need to create a class that inherits from
// ContentItemAsyncEditorProviderBase<TProvider> where TProvider is the type of your class.
public class EmployeeAsyncEditorProvider : ContentItemAsyncEditorProviderBase<EmployeeAsyncEditorProvider>
{
    public EmployeeAsyncEditorProvider(IContentItemAsyncEditorProviderServices<EmployeeAsyncEditorProvider> providerServices)
        : base(providerServices)
    {
    }

    // This method determines if the editor provider can handle the content type or not.
    protected override bool CanHandleContentType(string contentType) => contentType == ContentTypes.Employee;

    // Here you can describe what editors you want to show in your async editor.
    public override Task<IEnumerable<AsyncEditorGroupDescriptor<ContentItem>>> DescribeEditorGroupsAsync() =>
        Task.FromResult(new[]
        {
            // The first group is simple, the editor group ID is PersonalDetails, the menu item title is Personal
            // Details. The editor group is not a publish group, so you won't be able to publish the content item on
            // this page.
            DescribeEditorGroup(PersonalDetails, T["Personal Details"]),
            // The Employment Details page is a publish group.
            DescribeEditorGroup(EmploymentDetails, T["Employment Details"], isPublishGroup: true),
        }.AsEnumerable());
}

// NEXT STATION: Models/AsyncEditorEmployeePart.cs
