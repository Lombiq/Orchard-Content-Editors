using Lombiq.ContentEditors.Models;
using Orchard.Localization;

namespace Lombiq.ContentEditors.Services
{
    public abstract class EditorGroupsProviderBase : IEditorGroupsProvider
    {
        public Localizer T { get; set; }


        public EditorGroupsProviderBase()
        {
            T = NullLocalizer.Instance;
        }


        public abstract bool CanProvideEditorGroups(string contentType);

        public abstract EditorGroupsSettings GetEditorGroupsSettings();
    }
}