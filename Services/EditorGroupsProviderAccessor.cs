using System.Collections.Generic;
using System.Linq;
using Lombiq.EditorGroups.Models;

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
            _editorGroupsProviders.FirstOrDefault(provider => provider.CanProvideEditorGroups(contentType)) ?? 
            new DefaultEditorGroupsProvider();



        private class DefaultEditorGroupsProvider : IEditorGroupsProvider
        {
            public bool CanProvideEditorGroups(string contentType) => false;

            public EditorGroupsSettings GetEditorGroupsSettings() =>
                new EditorGroupsSettings
                {
                    EditorGroups = Enumerable.Empty<EditorGroupDescriptor>()
                };
        }
    }
}