using Lombiq.EditorGroups.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;

namespace Lombiq.EditorGroups.Drivers
{
    public class AsyncEditorPartDriver : ContentPartDriver<AsyncEditorPart>
    {
        protected override DriverResult Editor(AsyncEditorPart part, dynamic shapeHelper) => 
            ContentShape("Parts_EditorGroups_Edit",() => 
                shapeHelper.EditorTemplate(
                    TemplateName: "Parts/EditorGroups",
                    Model: part,
                    Prefix: Prefix));

        protected override void Exporting(AsyncEditorPart part, ExportContentContext context) => 
            ExportInfoset(part, context);

        protected override void Importing(AsyncEditorPart part, ImportContentContext context) => 
            ImportInfoset(part, context);
    }
}