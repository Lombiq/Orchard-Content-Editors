using Lombiq.ContentEditors.Constants;
using Lombiq.ContentEditors.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using OrchardCore.Admin;
using OrchardCore.Modules;

namespace Lombiq.ContentEditors.Controllers;

[Feature(FeatureIds.AsyncEditor)]
[Admin]
[Route(Routes.ContentItemAsyncEditor)]
public sealed class ContentItemAsyncEditorController : Controller
{
    [HttpGet("{providerName}/{contentType}/{contentItemId?}")]
    [ScriptUnsafeEval]
    public ActionResult Index(string providerName, string contentType, string contentItemId)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        return View(new ContentItemAsyncEditorViewModel
        {
            ProviderName = providerName,
            ContentType = contentType,
            ContentItemId = contentItemId,
        });
    }
}
