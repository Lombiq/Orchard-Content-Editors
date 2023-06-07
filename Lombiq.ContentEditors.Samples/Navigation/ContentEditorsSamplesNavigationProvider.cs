using Lombiq.ContentEditors.Controllers;
using Lombiq.ContentEditors.Samples.Constants;
using Lombiq.ContentEditors.Samples.Controllers;
using Lombiq.ContentEditors.Samples.Services;
using Lombiq.HelpfulLibraries.OrchardCore.Navigation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using OrchardCore.Navigation;

namespace Lombiq.ContentEditors.Samples.Navigation;

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
                .Add(T["Support Ticket (front-end)"], itemBuilder => itemBuilder
                    .Action<FrontEndDemoContentItemAsyncEditorController>(context, controller => controller.Index(null)))
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
