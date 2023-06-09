using Lombiq.ContentEditors.Samples.Constants;
using Lombiq.ContentEditors.Samples.Services;
using Lombiq.ContentEditors.ViewModels;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.Modules;

namespace Lombiq.ContentEditors.Samples.Controllers;

// In case you want to create an async editor UI on a page other than the admin UI, you can create a similar controller
// to this one.
[Feature(FeatureIds.Samples)]
[Route(Routes.FrontEndContentItemAsyncEditor)]
public class FrontEndDemoContentItemAsyncEditorController : Controller
{
    [HttpGet("{contentItemId?}")]
    public ActionResult Index(string contentItemId) =>
        // You can use the existing ContentItemAsyncEditorViewModel to pass the required data.
        View(new ContentItemAsyncEditorViewModel
        {
            ProviderName = nameof(SupportTicketAsyncEditorProvider),
            ContentType = ContentTypes.SupportTicket,
            ContentItemId = contentItemId,
        });
}

// NEXT STATION: Views/FrontEndDemoContentItemAsyncEditor/Index.cshtml
