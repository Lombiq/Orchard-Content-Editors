using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Lombiq.ContentEditors.Controllers
{
    public class AsyncEditorDemoController : Controller
    {
        [Route("demo/async-editor")]
        public ActionResult Index()
        {
            if (!HttpContext.IsDevelopmentAndLocalhost()) return NotFound();

            return View();
        }
    }
}
