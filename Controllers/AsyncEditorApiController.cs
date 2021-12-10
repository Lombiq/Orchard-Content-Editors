using Lombiq.ContentEditors.Constants;
using Lombiq.ContentEditors.Dtos;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.ModelBinding;
using System;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Lombiq.ContentEditors.Controllers
{
    [Route(Routes.AsyncEditorApi + "/{id}/{editorGroup?}")]
    public class AsyncEditorApiController : Controller
    {
        private readonly IContentItemDisplayManager _contentItemDisplayManager;
        private readonly IDisplayHelper _displayHelper;
        private readonly IContentManager _contentManager;
        private readonly IUpdateModelAccessor _updateModelAccessor;
        private readonly IShapeFactory _shapeFactory;

        public AsyncEditorApiController(
            IContentItemDisplayManager contentItemDisplayManager,
            IDisplayHelper displayHelper,
            IContentManager contentManager,
            IUpdateModelAccessor updateModelAccessor,
            IShapeFactory shapeFactory)
        {
            _contentItemDisplayManager = contentItemDisplayManager;
            _displayHelper = displayHelper;
            _contentManager = contentManager;
            _updateModelAccessor = updateModelAccessor;
            _shapeFactory = shapeFactory;
        }

        [HttpGet]
        public async Task<ActionResult<AsyncEditorGroupDto>> GetEditorGroup(string id, string editorGroup)
        {
            var item = await _contentManager.GetAsync(id);
            var editorShape = await _contentItemDisplayManager.BuildEditorAsync(
                item,
                _updateModelAccessor.ModelUpdater,
                false,
                editorGroup);
            var shape = await _shapeFactory.CreateAsync("AsyncEditor_Content", new { EditorShape = editorShape });
            var editorContent = await _displayHelper.ShapeExecuteAsync(shape);
            await using var stringWriter = new StringWriter();
            editorContent.WriteTo(stringWriter, HtmlEncoder.Default);

            return Ok(new AsyncEditorGroupDto
            {
                EditorGroup = editorGroup,
                EditorHtml = stringWriter.ToString(),
            });
        }

        [HttpPost]
        public async Task<ActionResult> SaveEditorGroup(string id, string editorGroup)
        {
            var item = await _contentManager.GetAsync(id);
            await _contentItemDisplayManager.UpdateEditorAsync(item, _updateModelAccessor.ModelUpdater, false, editorGroup);

            if (ModelState.IsValid)
            {
                await _contentManager.UpdateAsync(item);
            }

            return ModelState.IsValid
                ? Ok()
                : BadRequest(ModelState);
        }
    }
}
