﻿using Lombiq.ContentEditors.Models;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;

namespace Lombiq.ContentEditors.Drivers
{
    public class AsyncEditorPartDriver : ContentPartDriver<AsyncEditorPart>
    {
        protected override DriverResult Editor(AsyncEditorPart part, dynamic shapeHelper) => 
            ContentShape("Parts_AsyncEditor_Edit", () => shapeHelper.Lombiq_AsyncEditorLoader());

        protected override void Exporting(AsyncEditorPart part, ExportContentContext context) => 
            ExportInfoset(part, context);

        protected override void Importing(AsyncEditorPart part, ImportContentContext context) => 
            ImportInfoset(part, context);

        protected override void Cloning(AsyncEditorPart originalPart, AsyncEditorPart clonePart, CloneContentContext context) =>
            this.CloneInfoset(originalPart, clonePart);
    }
}