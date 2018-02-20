using Lombiq.EditorGroups.Models;
using Orchard.ContentManagement;
using Orchard.DisplayManagement.Descriptors;

namespace Lombiq.EditorGroups.Providers
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
                    var shape = displaying.Shape;

                    var contentItem = displaying.Shape.ContentItem as ContentItem;
                    if (contentItem == null) return;

                    var contentTypeDefinition = contentItem.TypeDefinition;
                    if (!contentTypeDefinition.Settings.TryGetValue("Stereotype", out var stereotype))
                    {
                        stereotype = "Content";
                    }

                    var asyncEditorShapeType = stereotype + "_Edit_AsyncEditor";

                    var editorGroupsPart = contentItem.As<EditorGroupsPart>();
                    if (editorGroupsPart == null || !editorGroupsPart.AsyncEditorContext) return;

                    displaying.ShapeMetadata.Wrappers.Add("AsyncEditor_Wrapper");

                    // [Stereotype]_Edit_AsyncEditor e.g. Content.Edit.AsyncEditor
                    displaying.ShapeMetadata.Alternates.Add(asyncEditorShapeType);

                    // [Stereotype]_Edit_AsyncEditor__[ContentType] e.g. Content.Edit.AsyncEditor-MyContentType
                    displaying.ShapeMetadata.Alternates.Add(asyncEditorShapeType + "__" + contentItem.ContentType);
                });
        }
    }
}