# Lombiq Content Editors for Orchard Core

[![Lombiq.ContentEditors NuGet](https://img.shields.io/nuget/v/Lombiq.ContentEditors?label=Lombiq.ContentEditors)](https://www.nuget.org/packages/Lombiq.ContentEditors/) [![Lombiq.ContentEditors.Samples NuGet](https://img.shields.io/nuget/v/Lombiq.ContentEditors.Samples?label=Lombiq.ContentEditors.Samples)](https://www.nuget.org/packages/Lombiq.ContentEditors.Samples/) [![Lombiq.ContentEditors.Tests.UI NuGet](https://img.shields.io/nuget/v/Lombiq.ContentEditors.Tests.UI?label=Lombiq.ContentEditors.Tests.UI)](https://www.nuget.org/packages/Lombiq.ContentEditors.Tests.UI/)

## About

Module for managing advanced content editing processes based on editor groups and asynchronous operations.

Do you want to quickly try out this project and see it in action? Check it out in our [Open-Source Orchard Core Extensions](https://github.com/Lombiq/Open-Source-Orchard-Core-Extensions) full Orchard Core solution and also see our other useful Orchard Core-related open-source projects!

## Documentation

### Async Editor

The Async Editor feature provides an infrastructure for creating editors that can load and save content asynchronously. The editor can optionally render multiple pages where each page can load and save data independently.

#### Creating an Async Editor for content items

To create an content type-specific async editor you need to create a provider class that inherits from `ContentItemAsyncEditorProviderBase<TProvider>` where `TProvider` is the name of your provider class. Example:

    ```csharp
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
    ```

The `CanHandleContentType` method is used to determine if the provider can handle the content type.

The `DescribeEditorGroupsAsync` method is used to describe the available editor groups. The above example describes two groups: `PersonalDetails` and `EmploymentDetails`. A group can be marked as the publish group meaning the user can publish the content item only on that editor page. The `DescribeEditorGroup` has two other parameters: `isAccessibleFactory` and `isFilledFactory`. These can be used to determine if the editor group is accessible (e.g., the user has permission to edit that group) and if the group is filled with data (e.g., the fill state is tracked somehow).

Once the Content Item asnyc editor provider is implemented you can access it on the Admin UI using this route: `Admin/ContentItemAsyncEditor/{providerName}/{contentType}/{contentItemId?}` e.g., `Admin/ContentItemAsyncEditor/EmployeeAsyncEditorProvider/Employee`.

#### Displaying async editors on the front-end

If you want to display the async editor on a page that is not specific to the Admin UI, you'll need to create a controller for it. Example:

    ```csharp
    public class FrontEndDemoContentItemAsyncEditorController : Controller
    {
        [HttpGet("{contentItemId?}")]
        public ActionResult Index(string contentItemId) =>
            View(new ContentItemAsyncEditorViewModel
            {
                ProviderName = nameof(SupportTicketAsyncEditorProvider),
                ContentType = ContentTypes.SupportTicket,
                ContentItemId = contentItemId,
            });
    }
    ```

Then, in the view you can use the `ContentItemAsyncEditor` shape to render the editor. Example:

    ```html
    <shape type="ContentItemAsyncEditor"
       prop-providerName="@Model.ProviderName"
       prop-contentType="@Model.ContentType"
       prop-contentItemId="@Model.ContentItemId">
    </shape>
    ```

## Dependencies

This module has the following dependencies:

- [Lombiq Helpful Libraries for Orchard Core](https://github.com/Lombiq/Helpful-Libraries)
- [Lombiq Node.js Extensions](https://gihub.com/Lombiq/NodeJs-Extensions)

## Contributing and support

Bug reports, feature requests, comments, questions, code contributions and love letters are warmly welcome. You can send them to us via GitHub issues and pull requests. Please adhere to our [open-source guidelines](https://lombiq.com/open-source-guidelines) while doing so.

This project is developed by [Lombiq Technologies](https://lombiq.com/). Commercial-grade support is available through Lombiq.
