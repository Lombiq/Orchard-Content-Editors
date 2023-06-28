using Lombiq.ContentEditors.Controllers;
using Lombiq.ContentEditors.Samples.Constants;
using Lombiq.ContentEditors.Samples.Controllers;
using Lombiq.ContentEditors.Samples.Services;
using Lombiq.HelpfulLibraries.OrchardCore.Navigation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using OrchardCore.Navigation;

namespace Lombiq.ContentEditors.Samples.Navigation;

// This is the navigation provider for the Content Editors Samples module. It adds menu items to a navigation with an ID
// of "MainMenu". If you are using the Lombiq Base Theme then this navigation is rendered in the header. If you are
// using the OSOCE app then this theme is enabled by default.
public class ContentEditorsSamplesNavigationProvider : MainMenuNavigationProviderBase
{
    public ContentEditorsSamplesNavigationProvider(
        IHttpContextAccessor hca,
        IStringLocalizer<ContentEditorsSamplesNavigationProvider> stringLocalizer)
        : base(hca, stringLocalizer)
    {
    }

    protected override void Build(NavigationBuilder builder)
    {
        var context = _hca.HttpContext;
        builder
            .Add(T["Content Editors Samples"], builder => builder
                // It adds a menu item with a link to the front-end async editor demo page.
                // Alternatively you can open the page using this URL: /FrontEndContentItemAsyncEditor
                .Add(T["Support Ticket (front-end)"], itemBuilder => itemBuilder
                    .ActionTask<FrontEndDemoContentItemAsyncEditorController>(context, controller => controller.Index(null)))
                // It adds a menu item with a link to the content item async editor demo page.
                // Alternatively you can open the page using this URL: /Admin/ContentItemAsyncEditor/EmployeeAsyncEditorProvider/Employee
                .Add(T["Employee (admin)"], itemBuilder => itemBuilder
                    .Action<ContentItemAsyncEditorController>(
                        _hca.HttpContext,
                        controller => controller.Index(
                            nameof(EmployeeAsyncEditorProvider),
                            ContentTypes.Employee,
                            null)))
            );
    }
}

// END OF TRAINING SECTION: Content item async editor
// END OF TRAINING SECTION: Front-end async editor
