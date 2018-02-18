using Lombiq.EditorGroups.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;

namespace Lombiq.EditorGroups.Drivers
{
    public class EditorGroupsPartDriver : ContentPartDriver<EditorGroupsPart>
    {
        protected override DriverResult Editor(EditorGroupsPart part, dynamic shapeHelper) => 
            ContentShape("Parts_EditorGroups_Edit",() => 
                shapeHelper.EditorTemplate(
                    TemplateName: "Parts/EditorGroups",
                    Model: part,
                    Prefix: Prefix));

        protected override DriverResult Editor(EditorGroupsPart part, IUpdateModel updater, dynamic shapeHelper)
        {
            updater.TryUpdateModel(part, Prefix, null, null);

            return Editor(part, shapeHelper);
        }

        protected override void Exporting(EditorGroupsPart part, ExportContentContext context) => 
            ExportInfoset(part, context);

        protected override void Importing(EditorGroupsPart part, ImportContentContext context) => 
            ImportInfoset(part, context);
    }
}