using Lombiq.ContentEditors.Models;
using Orchard;

namespace Lombiq.ContentEditors.Services
{
    public interface IEditorGroupsProvider : IDependency
    {
        /// <summary>
        /// Checks if the provider is for the given content type.
        /// </summary>
        /// <param name="contentType">Content type to be checked if this is for the current provider.</param>
        /// <returns>True if the provider is for the given type.</returns>
        bool CanProvideEditorGroups(string contentType);

        /// <summary>
        /// Returns the editor group settings for the content type that the actual provider is for.
        /// </summary>
        /// <returns>Editor gorup settings.</returns>
        EditorGroupsSettings GetEditorGroupsSettings();
    }
}