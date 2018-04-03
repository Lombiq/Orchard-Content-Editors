using Lombiq.ContentEditors.Models;
using Orchard.Localization;

namespace Lombiq.ContentEditors.Services
{
    public abstract class EditorGroupsProviderBase
    {
        public Localizer T { get; set; }


        public EditorGroupsProviderBase()
        {
            T = NullLocalizer.Instance;
        }


        public abstract bool CanProvideEditorGroups(string contentType);

        public abstract EditorGroupsSettings GetEditorGroupsSettings();


        protected virtual EditorGroupDescriptor EditorGroup(string name, LocalizedString title, bool isPublishGroup = false) =>
            new EditorGroupDescriptor { Name = name, Title = title.Text, IsPublishGroup = isPublishGroup };
    }
}