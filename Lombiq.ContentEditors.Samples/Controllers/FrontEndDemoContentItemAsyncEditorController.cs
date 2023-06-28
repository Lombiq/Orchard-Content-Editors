using Lombiq.ContentEditors.Samples.Constants;
using Lombiq.ContentEditors.Samples.Services;
using Lombiq.ContentEditors.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.Contents;
using OrchardCore.Modules;
using System.Threading.Tasks;

namespace Lombiq.ContentEditors.Samples.Controllers;

// In case you want to create an async editor UI on a page other than the admin UI, you can create a similar controller
// to this one.
[Feature(FeatureIds.Samples)]
[Route(Routes.FrontEndContentItemAsyncEditor)]
public class FrontEndDemoContentItemAsyncEditorController : Controller
{
    private readonly IAuthorizationService _authorizationService;

    public FrontEndDemoContentItemAsyncEditorController(IAuthorizationService authorizationService) =>
        _authorizationService = authorizationService;

    [HttpGet("{contentItemId?}")]
    public async Task<ActionResult> Index(string contentItemId)
    {
        if (!await _authorizationService.AuthorizeAsync(User, Permissions.EditContent))
            return Unauthorized();

        // You can use the existing ContentItemAsyncEditorViewModel to pass the required data.
        return View(
            new ContentItemAsyncEditorViewModel
            {
                ProviderName = nameof(SupportTicketAsyncEditorProvider),
                ContentType = ContentTypes.SupportTicket,
                ContentItemId = contentItemId,
            });
    }
}

// NEXT STATION: Views/FrontEndDemoContentItemAsyncEditor/Index.cshtml
