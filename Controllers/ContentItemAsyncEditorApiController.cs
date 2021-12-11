using Lombiq.ContentEditors.Constants;
using Lombiq.ContentEditors.Dtos;
using Lombiq.ContentEditors.Models;
using Lombiq.ContentEditors.Services;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.ContentEditors.Controllers
{

    [Route(Routes.ContentItemAsyncEditorApi)]
    public class ContentItemAsyncEditorApiController : Controller
    {
        private readonly IEnumerable<IAsyncEditorProvider<ContentItem>> _providers;
        private readonly IContentManager _contentManager;

        public ContentItemAsyncEditorApiController(
            IEnumerable<IAsyncEditorProvider<ContentItem>> providers,
            IContentManager contentManager)
        {
            _providers = providers;
            _contentManager = contentManager;
        }

        [HttpGet]
        public async Task<ActionResult<AsyncEditorGroupDto>> Get([FromQuery] RenderContentItemAsyncEditorDto dto)
        {
            var provider = GetProvider(dto.ProviderName);
            if (provider == null) return NotFound();

            var item = await GetOrCreateAsync(dto.ContentType, dto.ContentItemId);
            if (item == null) return NotFound();

            var context = PopulateContext(dto, item);
            if (!await provider.CanRenderEditorGroupAsync(context)) return NotFound();

            return await AsyncEditorResultAsync(context, provider);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Post([FromQuery] SubmitContentItemAsyncEditorDto dto)
        {
            var provider = GetProvider(dto.ProviderName);
            if (provider == null) return NotFound();

            var item = await GetOrCreateAsync(dto.ContentType, dto.ContentItemId);
            if (item == null) return NotFound();

            var context = PopulateContext(dto, item);
            if (!await provider.CanRenderEditorGroupAsync(context)) return NotFound();

            var modelState = await provider.UpdateEditorAsync(context);
            if (!modelState.IsValid)
            {
                return await AsyncEditorResultAsync(context, provider);
            }

            if (string.IsNullOrEmpty(dto.NextEditorGroup) || dto.NextEditorGroup == dto.EditorGroup)
            {
                return await AsyncEditorResultAsync(context, provider);
            }

            var nextEditorContext = PopulateContext(dto, item, dto.NextEditorGroup);
            return await AsyncEditorResultAsync(
                !await provider.CanRenderEditorGroupAsync(nextEditorContext) ? context : nextEditorContext,
                provider);

        }

        private IAsyncEditorProvider<ContentItem> GetProvider(string name) =>
            _providers.FirstOrDefault(provider => provider.Name == name);

        private Task<ContentItem> GetOrCreateAsync(string contentType, string contentItemId)
        {
            if (string.IsNullOrEmpty(contentType))
            {
                return _contentManager.GetAsync(contentItemId);
            }

            return string.IsNullOrEmpty(contentItemId)
                ? _contentManager.NewAsync(contentType)
                : _contentManager.GetAsync(contentItemId);
        }

        private static AsyncEditorContext<ContentItem> PopulateContext(
            RenderContentItemAsyncEditorDto dto,
            ContentItem contentItem,
            string editorGroup = null) =>
            new()
            {
                Content = contentItem,
                EditorGroup = editorGroup ?? dto.EditorGroup,
                AsyncEditorId = dto.AsyncEditorId,
            };

        private async Task<OkObjectResult> AsyncEditorResultAsync(
            AsyncEditorContext<ContentItem> context,
            IAsyncEditorProvider<ContentItem> provider,
            string editorGroup = null)
        {
            var renderedEditor = await provider.RenderEditorGroupAsync(context);
            var editorGroups = await provider.GetOrderedEditorGroupsAsync(context);

            return Ok(new AsyncEditorGroupDto
            {
                EditorGroup = editorGroup ?? context.EditorGroup,
                EditorGroups = editorGroups,
                EditorHtml = renderedEditor,
            });
        }
    }
}
