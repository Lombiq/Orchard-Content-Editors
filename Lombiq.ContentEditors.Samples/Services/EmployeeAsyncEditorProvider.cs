using Lombiq.ContentEditors.Models;
using Lombiq.ContentEditors.Samples.Constants;
using Lombiq.ContentEditors.Services;
using OrchardCore.ContentManagement;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Lombiq.ContentEditors.Samples.Constants.EditorGroups.Employee;

namespace Lombiq.ContentEditors.Samples.Services;

// If you want to create an async editor for your content type then you need to create a class that inherits from
// ContentItemAsyncEditorProviderBase<TProvider> where TProvider is the type of your class.
public class EmployeeAsyncEditorProvider : ContentItemAsyncEditorProviderBase<EmployeeAsyncEditorProvider>
{
    public EmployeeAsyncEditorProvider(IContentItemAsyncEditorProviderServices<EmployeeAsyncEditorProvider> providerServices)
        : base(providerServices)
    {
    }

    // This method determines if the editor provider can handle the content type or not.
    protected override bool CanHandleContentType(string contentType) => contentType == ContentTypes.Employee;

    // Here you can describe what editors you want to show in your async editor. These editor groups are Orchard Core
    // editor groups containing the editor shapes.
    public override Task<IEnumerable<AsyncEditorGroupDescriptor<ContentItem>>> DescribeEditorGroupsAsync() =>
        Task.FromResult(new[]
        {
            // The first group is simple, the editor group ID is PersonalDetails, the menu item title is Personal
            // Details. A group can be marked as the publish group, meaning the user can publish the content item only on
            // that editor page. The DescribeEditorGroup has two other parameters: isAccessibleFactory and
            // isFilledFactory. These can be used to determine if the editor group is accessible (e.g., the user has
            // permission to edit that group) and if the group is filled with data (e.g., the fill state is tracked
            // somehow).
            DescribeEditorGroup(PersonalDetails, T["Personal Details"]),
            // The Employment Details is the other group for the Employee async editor. This is the publish group.
            DescribeEditorGroup(EmploymentDetails, T["Employment Details"], isPublishGroup: true),
        }.AsEnumerable());
}

// NEXT STATION: Models/AsyncEditorEmployeePart.cs
