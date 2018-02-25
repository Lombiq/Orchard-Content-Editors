using System.Collections.Generic;
using System.Linq;
using Lombiq.EditorGroups.Models;

namespace Lombiq.EditorGroups.Services
{
    public class EditorGroupsSettingsProviderAccessor : IEditorGroupsProviderAccessor
    {
        private readonly IEnumerable<IEditorGroupsProvider> _editorGroupsProviders;


        public EditorGroupsSettingsProviderAccessor(IEnumerable<IEditorGroupsProvider> editorGroupsProviders)
        {
            _editorGroupsProviders = editorGroupsProviders;
        }


        public IEditorGroupsProvider GetProvider(string contentType) =>
            _editorGroupsProviders.FirstOrDefault(provider => provider.CanProvideEditorGroups(contentType));
    }
}