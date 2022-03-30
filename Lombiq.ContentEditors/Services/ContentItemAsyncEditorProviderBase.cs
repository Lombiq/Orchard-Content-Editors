using Lombiq.ContentEditors.Extensions;
using Lombiq.ContentEditors.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display;
using OrchardCore.Contents;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.ModelBinding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Lombiq.ContentEditors.Services;

public abstract class ContentItemAsyncEditorProviderBase<TProvider> : IAsyncEditorProvider<ContentItem>
    where TProvider : IAsyncEditorProvider<ContentItem>
{
    protected readonly IContentManager _contentManager;
    protected readonly IContentItemDisplayManager _contentItemDisplayManager;
    protected readonly IDisplayHelper _displayHelper;
    protected readonly IShapeFactory _shapeFactory;
    protected readonly IUpdateModelAccessor _updateModelAccessor;
    protected readonly IStringLocalizer<TProvider> T;
    protected readonly IAuthorizationService _authorizationService;
    protected readonly IHttpContextAccessor _hca;

    public virtual string Name => typeof(TProvider).Name;

    protected ContentItemAsyncEditorProviderBase(IContentItemAsyncEditorProviderServices<TProvider> providerServices)
    {
        _contentManager = providerServices.ContentManager.Value;
        _contentItemDisplayManager = providerServices.ContentItemDisplayManager.Value;
        _displayHelper = providerServices.DisplayHelper.Value;
        _shapeFactory = providerServices.ShapeFactory.Value;
        _updateModelAccessor = providerServices.UpdateModelAccessor.Value;
        T = providerServices.StringLocalizer.Value;
        _authorizationService = providerServices.AuthorizationService.Value;
        _hca = providerServices.HttpContextAccessor.Value;
    }

    public abstract Task<IEnumerable<AsyncEditorGroupDescriptor<ContentItem>>> DescribeEditorGroupsAsync();

    protected virtual bool CanHandleContentType(string contentType) => true;

    protected virtual async Task<bool> CanRenderEditorGroupAsync(AsyncEditorContext<ContentItem> context, string editorGroup)
    {
        if (!CanHandleContentType(context.Content.ContentType)) return false;

        if (!await AuthorizeEditorGroupAsync(context)) return false;

        var descriptors = (await DescribeEditorGroupsAsync()).Select(descriptor => descriptor.Name).ToList();
        var previousIndex = descriptors.IndexOf(editorGroup ?? context.EditorGroup) - 1;
        return previousIndex < 0 || context.Content.IsEditorGroupFilled(context.AsyncEditorId, descriptors[previousIndex]);
    }

    public virtual async Task<string> RenderEditorAsync(AsyncEditorContext<ContentItem> context)
    {
        await ThrowIfGroupIsInvalidAsync(context);

        var editorShape = await _contentItemDisplayManager.BuildEditorAsync(
            context.Content,
            _updateModelAccessor.ModelUpdater,
            context.Content.IsNew(),
            context.EditorGroup);

        AddEditorShapeAlternates(context, editorShape);

        return await WrapAndRenderShapeAsync(editorShape);
    }

    public virtual async Task<AsyncEditorUpdateResult> UpdateEditorAsync(AsyncEditorContext<ContentItem> context)
    {
        await ThrowIfGroupIsInvalidAsync(context);

        var editorShape = await _contentItemDisplayManager.UpdateEditorAsync(
            context.Content,
            _updateModelAccessor.ModelUpdater,
            context.Content.IsNew(),
            context.EditorGroup);

        AddEditorShapeAlternates(context, editorShape);

        if (!_updateModelAccessor.ModelUpdater.ModelState.IsValid)
        {
            return CreateUpdateResult(editorShape, _updateModelAccessor.ModelUpdater.ModelState);
        }

        context.Content.SetFilledEditorGroup(context.AsyncEditorId, context.EditorGroup);

        await _contentManager.CreateOrUpdateAsync(context.Content);

        if (!(await DescribeEditorGroupsAsync()).First(group => @group.Name == context.EditorGroup).IsPublishGroup)
        {
            return CreateUpdateResult(editorShape, _updateModelAccessor.ModelUpdater.ModelState);
        }

        await _contentManager.PublishAsync(context.Content);

        return CreateUpdateResult(
            editorShape,
            _updateModelAccessor.ModelUpdater.ModelState,
            T["Editor has been successfully submitted."]);
    }

    protected virtual Task<bool> AuthorizeEditorGroupAsync(AsyncEditorContext<ContentItem> context) =>
        _authorizationService.AuthorizeAsync(_hca.HttpContext.User, CommonPermissions.EditContent, context.Content);

    protected virtual async Task<string> WrapAndRenderShapeAsync(IShape editorShape)
    {
        var shape = await _shapeFactory.CreateAsync("AsyncEditor_Editor", new { EditorShape = editorShape });
        var editorContent = await _displayHelper.ShapeExecuteAsync(shape);
        await using var stringWriter = new StringWriter();
        editorContent.WriteTo(stringWriter, HtmlEncoder.Default);

        return stringWriter.ToString();
    }

    protected async Task ThrowIfGroupIsInvalidAsync(AsyncEditorContext<ContentItem> context)
    {
        if (!await CanRenderEditorGroupAsync(context, context.EditorGroup))
        {
            throw new ArgumentException("The requested editor group is invalid.");
        }
    }

    protected Task<bool> IsEditorGroupFilledAsync(AsyncEditorContext<ContentItem> context, string editorGroup) =>
        Task.FromResult(context.Content.IsEditorGroupFilled(context.AsyncEditorId, editorGroup));

    protected virtual AsyncEditorGroupDescriptor<ContentItem> DescribeEditorGroup(
        string name,
        string displayText,
        bool isPublishGroup = false,
        Func<AsyncEditorContext<ContentItem>, ValueTask<bool>> isAccessibleFactory = null,
        Func<AsyncEditorContext<ContentItem>, ValueTask<bool>> isFilledFactory = null) =>
        new()
        {
            Name = name,
            DisplayText = displayText,
            IsPublishGroup = isPublishGroup,
            IsAccessibleAsync = isAccessibleFactory ??
                (async (context) => await CanRenderEditorGroupAsync(context, name)),
            IsFilledAsync = isFilledFactory ??
                (async (context) => await IsEditorGroupFilledAsync(context, name)),
        };

    protected virtual void AddEditorShapeAlternates(AsyncEditorContext<ContentItem> context, IShape editorShape)
    {
        editorShape.Metadata.Alternates.Add($"AsyncEditor_Content");
        editorShape.Metadata.Alternates.Add($"AsyncEditor_Content__{context.AsyncEditorId}");
        editorShape.Metadata.Alternates.Add($"AsyncEditor_Content__{context.EditorGroup}");
        editorShape.Metadata.Alternates.Add($"AsyncEditor_Content__{context.AsyncEditorId}__{context.EditorGroup}");
    }

    private AsyncEditorUpdateResult CreateUpdateResult(IShape editorShape, ModelStateDictionary modelState, string message = null) =>
        new()
        {
            ModelState = modelState,
            RenderedEditorShapeFactory = () => new ValueTask<string>(WrapAndRenderShapeAsync(editorShape)),
            Message = message,
        };
}