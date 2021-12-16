using Lombiq.ContentEditors.Constants;
using Lombiq.ContentEditors.ViewModels;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.Admin;
using OrchardCore.Modules;

namespace Lombiq.ContentEditors.Controllers
{
    [Feature(FeatureIds.AsyncEditor)]
    [Admin]
    [Route(Routes.ContentItemAsyncEditor)]
    public class ContentItemAsyncEditorController : Controller
    {
        [HttpGet("{providerName}/{contentType}/{contentItemId?}")]
        public ActionResult Index(string providerName, string contentType, string contentItemId) =>
            View(new ContentItemAsyncEditorViewModel
            {
                ProviderName = providerName,
                ContentType = contentType,
                ContentItemId = contentItemId,
            });
    }
}
