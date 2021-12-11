using Lombiq.ContentEditors.Extensions;
using Lombiq.ContentEditors.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Localization;
using OrchardCore.ContentManagement;
using OrchardCore.ContentManagement.Display;
using OrchardCore.DisplayManagement;
using OrchardCore.DisplayManagement.ModelBinding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace Lombiq.ContentEditors.Services
{
    public abstract class ContentItemAsyncEditorProviderBase<TProvider> : IAsyncEditorProvider<ContentItem>
        where TProvider : IAsyncEditorProvider<ContentItem>
    {
        protected readonly IContentManager _contentManager;
        protected readonly IContentItemDisplayManager _contentItemDisplayManager;
        protected readonly IDisplayHelper _displayHelper;
        protected readonly IShapeFactory _shapeFactory;
        protected readonly IUpdateModelAccessor _updateModelAccessor;
        protected readonly IStringLocalizer<TProvider> T;

        public virtual string Name => typeof(TProvider).Name;

        protected ContentItemAsyncEditorProviderBase(IContentItemAsyncEditorProviderServices<TProvider> providerServices)
        {
            _contentManager = providerServices.ContentManager.Value;
            _contentItemDisplayManager = providerServices.ContentItemDisplayManager.Value;
            _displayHelper = providerServices.DisplayHelper.Value;
            _shapeFactory = providerServices.ShapeFactory.Value;
            _updateModelAccessor = providerServices.UpdateModelAccessor.Value;
            T = providerServices.StringLocalizer.Value;
        }

        public abstract Task<IEnumerable<AsyncEditorGroup>> GetOrderedEditorGroupsAsync(AsyncEditorContext<ContentItem> context);

        public virtual async Task<bool> CanRenderEditorGroupAsync(AsyncEditorContext<ContentItem> context) =>
            (await GetOrderedEditorGroupsAsync(context)).Any(group => group.IsAccessible && group.Name == context.EditorGroup);

        public virtual async Task<string> RenderEditorGroupAsync(AsyncEditorContext<ContentItem> context)
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

            if ((await GetOrderedEditorGroupsAsync(context)).First(group => group.Name == context.EditorGroup)
                .IsPublishGroup)
            {
                await _contentManager.PublishAsync(context.Content);
            }

            return CreateUpdateResult(editorShape, _updateModelAccessor.ModelUpdater.ModelState);
        }

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
            if (!await CanRenderEditorGroupAsync(context))
            {
                throw new ArgumentException("The requested editor group is invalid.");
            }
        }

        protected virtual AsyncEditorGroup CreateEditorGroup(
            AsyncEditorContext<ContentItem> context,
            string name,
            string displayText,
            bool isAccessible = true) =>
            new()
            {
                Name = name,
                DisplayText = displayText,
                IsAccessible = isAccessible,
                IsFilled = context.Content.HasFilledEditorGroup(context.AsyncEditorId, name),
            };

        protected virtual void AddEditorShapeAlternates(AsyncEditorContext<ContentItem> context, IShape editorShape)
        {
            editorShape.Metadata.Alternates.Add($"AsyncEditor_Content");
            editorShape.Metadata.Alternates.Add($"AsyncEditor_Content__{context.AsyncEditorId}");
            editorShape.Metadata.Alternates.Add($"AsyncEditor_Content__{context.EditorGroup}");
            editorShape.Metadata.Alternates.Add($"AsyncEditor_Content__{context.AsyncEditorId}__{context.EditorGroup}");
        }

        private AsyncEditorUpdateResult CreateUpdateResult(IShape editorShape, ModelStateDictionary modelState) =>
            new()
            {
                ModelState = modelState,
                RenderedEditorShapeFactory = () => new ValueTask<string>(WrapAndRenderShapeAsync(editorShape)),
            };
    }
}
