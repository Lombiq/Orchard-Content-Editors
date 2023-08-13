using Lombiq.ContentEditors.Constants;
using Lombiq.ContentEditors.Models;
using Lombiq.ContentEditors.Services;
using Microsoft.AspNetCore.Mvc;
using OrchardCore.ContentManagement;
using OrchardCore.Modules;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Lombiq.ContentEditors.Controllers;

[Feature(FeatureIds.AsyncEditor)]
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
    public async Task<ActionResult<RenderedAsyncEditorGroupRequest>> Get([FromQuery] RenderAsyncEditorRequest request)
    {
        var provider = GetProvider(request.ProviderName);
        if (provider == null) return NotFound();

        var item = await _contentManager.GetOrCreateAsync(request.ContentId, request.ContentType, VersionOptions.Latest);
        if (item == null) return NotFound();

        var context = PopulateContext(request, item);
        if (!await provider.CanRenderEditorGroupAsync(context)) return NotFound();

        return await AsyncEditorResultAsync(context, provider);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Post([FromQuery] SubmitAsyncEditorRequest request)
    {
        var provider = GetProvider(request.ProviderName);
        if (provider == null) return NotFound();

        var item = await _contentManager.GetOrCreateAsync(request.ContentId, request.ContentType, VersionOptions.Latest);
        if (item == null) return NotFound();

        var context = PopulateContext(request, item);
        if (!await provider.CanRenderEditorGroupAsync(context)) return NotFound();

        var result = await provider.UpdateEditorAsync(context);
        if (!result.ModelState.IsValid ||
            string.IsNullOrEmpty(request.NextEditorGroup) ||
            request.NextEditorGroup == request.EditorGroup)
        {
            return await AsyncEditorResultAsync(
                context,
                provider,
                renderedEditor: await result.RenderedEditorShapeFactory(),
                message: result.Message);
        }

        var nextEditorContext = PopulateContext(request, item, request.NextEditorGroup);
        return await AsyncEditorResultAsync(
            !await provider.CanRenderEditorGroupAsync(nextEditorContext) ? context : nextEditorContext,
            provider,
            message: result.Message);
    }

    private IAsyncEditorProvider<ContentItem> GetProvider(string name) =>
        _providers.FirstOrDefault(provider => provider.Name == name);

    private static AsyncEditorContext<ContentItem> PopulateContext(
        RenderAsyncEditorRequest request,
        ContentItem contentItem,
        string editorGroup = null) =>
        new()
        {
            Content = contentItem,
            EditorGroup = editorGroup ?? request.EditorGroup,
            AsyncEditorId = request.AsyncEditorId,
        };

    private async Task<ViewResult> AsyncEditorResultAsync(
        AsyncEditorContext<ContentItem> context,
        IAsyncEditorProvider<ContentItem> provider,
        string editorGroup = null,
        string renderedEditor = null,
        string message = null)
    {
        renderedEditor ??= await provider.RenderEditorAsync(context);
        var editorGroups = await provider.DescribeEditorGroupsAsync();

        // Return ViewResult instead of simple Ok because the ModelState is not accessible in ad-hoc shapes hence
        // the validation summary wouldn't be rendered otherwise. Adding the validation summary HTML in the view.
        return View("Result", new RenderedAsyncEditorGroupRequest
        {
            ContentId = !context.Content.IsNew() ? context.Content.ContentItemId : null,
            EditorGroup = editorGroup ?? context.EditorGroup,
            EditorGroups = await Task.WhenAll(editorGroups.Select(async group => new AsyncEditorGroupRequest
            {
                Name = group.Name,
                DisplayText = group.DisplayText,
                IsPublishGroup = group.IsPublishGroup,
                IsAccessible = await group.IsAccessibleAsync(context),
                IsFilled = await group.IsFilledAsync(context),
            })),
            EditorHtml = renderedEditor,
            Message = message,
        });
    }
}
