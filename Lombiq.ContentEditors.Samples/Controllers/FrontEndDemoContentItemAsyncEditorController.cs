using Lombiq.ContentEditors.Samples.Constants;
using Lombiq.ContentEditors.Samples.Services;
using Lombiq.ContentEditors.ViewModels;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.Modules;

namespace Lombiq.ContentEditors.Samples.Controllers
{
    [Feature(FeatureIds.Samples)]
    [Route(Routes.FrontEndContentItemAsyncEditor)]
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
}
