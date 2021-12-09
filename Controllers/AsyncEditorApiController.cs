using Lombiq.ContentEditors.Constants;
using Lombiq.ContentEditors.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Lombiq.ContentEditors.Controllers
{
    [Route(Routes.AsyncEditorApi)]
    public class AsyncEditorApiController : ControllerBase
    {
        [HttpGet]
        public ActionResult<AsyncEditorGroupDto> GetEditorGroup()
        {
            return Ok(new AsyncEditorGroupDto
            {
                EditorGroup = "Test",
                EditorHtml = "<strong>test</strong>",
            });
        }
    }
}
