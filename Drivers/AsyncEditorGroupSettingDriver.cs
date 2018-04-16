using System.Collections.Generic;
using Lombiq.ContentEditors.Models;
using Lombiq.ContentEditors.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.MetaData;

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
        private readonly IAsyncEditorService _asyncEditorService;


        public AsyncEditorGroupSettingDriver(IAsyncEditorService asyncEditorService)
        {
            _asyncEditorService = asyncEditorService;
        }


        public DriverResult BuildDisplay(BuildDisplayContext context) => null;

        public DriverResult BuildEditor(BuildEditorContext context) => null;

        public void Cloned(CloneContentContext context)
        {
        }

        public void Cloning(CloneContentContext context)
        {
        }

        public void Exported(ExportContentContext context)
        {
        }

        public void Exporting(ExportContentContext context)
        {
        }

        public void GetContentItemMetadata(GetContentItemMetadataContext context)
        {
        }

        public IEnumerable<ContentPartInfo> GetPartInfo() =>
            new[]
            {
                new ContentPartInfo
                {
                    PartName = nameof(AsyncEditorPart),
                    Factory = typePartDefinition => new AsyncEditorPart { TypePartDefinition = typePartDefinition }
                }
            };

        public void ImportCompleted(ImportContentContext context)
        {
        }

        public void Imported(ImportContentContext context)
        {
        }

        public void Importing(ImportContentContext context)
        {
        }

        public DriverResult UpdateEditor(UpdateEditorContext context)
        {
            var asyncEditorPart = context.Content.As<AsyncEditorPart>();
            if (asyncEditorPart != null)
            {
                asyncEditorPart.CurrentEditorGroup = _asyncEditorService.GetEditorGroupDescriptor(asyncEditorPart, context.GroupId);
            }
            return null;
        }
    }
}