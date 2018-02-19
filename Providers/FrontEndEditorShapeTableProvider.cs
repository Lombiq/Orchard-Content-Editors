using Lombiq.EditorGroups.Models;
using Orchard;
using Orchard.ContentManagement;
using Orchard.DisplayManagement.Descriptors;
using Orchard.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Lombiq.EditorGroups.Providers
{
    public class FrontEndEditorShapeTableProvider : Component, IShapeTableProvider
    {
        public void Discover(ShapeTableBuilder builder)
        {
            builder
                .Describe("Content_Edit")
                //.Configure(descriptor =>
                //{
                //    descriptor.Wrappers.Add("AsyncEditor_Wrapper");
                //})
                //.OnCreated(created =>
                //{
                //    var shape = created.Shape;

                //    shape.Child.Add(created.New.PlaceChildContent(Source: shape));
                //})
                .OnDisplaying(displaying =>
                {
                    Logger.Error("Itt jártam geci.");
                    var shape = displaying.Shape;

                    var contentItem = displaying.Shape.ContentItem as ContentItem;

                    if (contentItem == null) return;

                    var editorGroupsPart = contentItem.As<EditorGroupsPart>();

                    if (editorGroupsPart == null) return;

                    // AsyncEditor__[ContentType] e.g. Widget-Blog
                    displaying.ShapeMetadata.Alternates.Add("AsyncEditor__" + contentItem.ContentType);
                });
        }
    }
}