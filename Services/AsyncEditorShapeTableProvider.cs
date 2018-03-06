using Lombiq.ContentEditors.Models;
using Orchard.ContentManagement;
using Orchard.DisplayManagement.Descriptors;
using Orchard.DisplayManagement.Implementation;
using System.Collections.Generic;

namespace Lombiq.ContentEditors.Services
{
    public class AsyncEditorShapeTableProvider : IShapeTableProvider
    {
        public void Discover(ShapeTableBuilder builder)
        {
            builder
                .Describe("Content_Edit")
                .OnCreated(created =>
                {
                    var shape = created.Shape;

                    shape.Child.Add(created.New.PlaceChildContent(Source: shape));
                })
                .OnDisplaying(displaying =>
                {
                    var asyncEditorPart = GetValidAsyncEditorPart(displaying);
                    if (asyncEditorPart == null) return;

                    var contentTypeDefinition = asyncEditorPart.TypeDefinition;
                    if (!contentTypeDefinition.Settings.TryGetValue("Stereotype", out var stereotype))
                    {
                        stereotype = "Content";
                    }

                    var asyncEditorShapeType = $"{stereotype}_Edit_Async";

                    displaying.ShapeMetadata.Wrappers.Add("AsyncEditor_Wrapper");

                    // [Stereotype]_Edit_AsyncEditor e.g. Content.Edit.AsyncEditor
                    displaying.ShapeMetadata.Alternates.Add(asyncEditorShapeType);

                    // [Stereotype]_Edit_AsyncEditor__[ContentType] e.g. Content.Edit.AsyncEditor-MyContentType
                    displaying.ShapeMetadata.Alternates.Add(
                        $"{asyncEditorShapeType}__{asyncEditorPart.ContentItem.ContentType}");

                    if (asyncEditorPart.CurrentEditorGroup != null)
                    {
                        // [Stereotype]_Edit_AsyncEditor__[ContentType]__[EditorGroup] e.g. Content.Edit.AsyncEditor-MyContentType-MyEditorGroup
                        displaying.ShapeMetadata.Alternates.Add(
                            $"{asyncEditorShapeType}__{asyncEditorPart.ContentItem.ContentType}__{asyncEditorPart.CurrentEditorGroup.Name}");
                    }
                });

            AddAlternatesToAsyncEditorShapes(
                builder,
                new[]
                {
                    "AsyncEditor_Actions",
                    "AsyncEditor_Editor",
                    "AsyncEditor_Title"
                });
        }


        private void AddAlternatesToAsyncEditorShapes(ShapeTableBuilder builder, IEnumerable<string> shapeTypes)
        {
            foreach (var shapeType in shapeTypes)
            {
                builder
                    .Describe(shapeType)
                    .OnDisplaying(displaying =>
                    {
                        var asyncEditorPart = GetValidAsyncEditorPart(displaying);
                        if (asyncEditorPart == null) return;

                        // [AsyncEditorShapeType]__[ContentType] e.g. AsyncEditor_Actions-MyContentType
                        displaying.ShapeMetadata.Alternates.Add(
                            $"{displaying.ShapeMetadata.Type}__{asyncEditorPart.ContentItem.ContentType}");
                    });
            }
        }

        private AsyncEditorPart GetValidAsyncEditorPart(ShapeDisplayingContext context)
        {
            var shape = context.Shape;

            var contentItem = context.Shape.ContentItem as ContentItem;
            if (contentItem == null) return null;

            var asyncEditorPart = contentItem.As<AsyncEditorPart>();
            if (asyncEditorPart == null || !asyncEditorPart.IsAsyncEditorContext) return null;

            return asyncEditorPart;
        }
    }
}