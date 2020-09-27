using Lombiq.ContentEditors.Models;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.MetaData;
using System.Collections.Generic;

namespace Lombiq.ContentEditors.Drivers
{
    /// <summary>
    /// Driver with the sole purpose of setting the current editor group in the <see cref="AsyncEditorPart"/> of the 
    /// currently updated content item. This can pretty much only done in such a driver implementation, as anything else
    /// (like a user-level handler) will run after drivers. It also can't be done in <see cref="AsyncEditorPartDriver"/>
    /// in an UpdateEditor() override, because it's explicitly implemented in the base class (see 
    /// <see href="https://stackoverflow.com/questions/5976216/how-to-call-an-explicitly-implemented-interface-method-on-the-base-class"/>). 
    /// </summary>
    public class AsyncEditorGroupSettingDriver : IContentPartDriver
    {
        public DriverResult BuildDisplay(BuildDisplayContext context) => null;

        public DriverResult BuildEditor(BuildEditorContext context) => null;

        public void Cloned(CloneContentContext context) { }

        public void Cloning(CloneContentContext context) { }

        public void Exported(ExportContentContext context) { }

        public void Exporting(ExportContentContext context) { }

        public void GetContentItemMetadata(GetContentItemMetadataContext context) { }

        public IEnumerable<ContentPartInfo> GetPartInfo() =>
            new[]
            {
                new ContentPartInfo
                {
                    PartName = nameof(AsyncEditorPart),
                    Factory = typePartDefinition => new AsyncEditorPart { TypePartDefinition = typePartDefinition }
                }
            };

        public void ImportCompleted(ImportContentContext context) { }

        public void Imported(ImportContentContext context) { }

        public void Importing(ImportContentContext context) { }

        public DriverResult UpdateEditor(UpdateEditorContext context)
        {
            var asyncEditorPart = context.Content.AsAsyncEditorPart();

            if (asyncEditorPart != null)
                asyncEditorPart.CurrentEditorGroup = asyncEditorPart.GetEditorGroupDescriptor(context.GroupId);

            return null;
        }
    }
}