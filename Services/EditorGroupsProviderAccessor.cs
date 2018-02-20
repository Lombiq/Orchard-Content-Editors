using System.Collections.Generic;
using System.Linq;

namespace Lombiq.EditorGroups.Services
{
    public class EditorGroupsProviderAccessor : IEditorGroupsProviderAccessor
    {
        private readonly IEnumerable<IEditorGroupsProvider> _editorGroupsProviders;


        public EditorGroupsProviderAccessor(IEnumerable<IEditorGroupsProvider> editorGroupsProviders)
        {
            _editorGroupsProviders = editorGroupsProviders;
        }


        public IEditorGroupsProvider GetProvider(string contentType) =>
            _editorGroupsProviders.FirstOrDefault(provider => provider.CanProvideEditorGroups(contentType));
    }
}