using Lombiq.EditorGroups.Models;
using Orchard;
using System.Collections.Generic;

namespace Lombiq.EditorGroups.Services
{
    public interface IEditorGroupsProvider : IDependency
    {
        bool CanProvideEditorGroups(string contentType);
        EditorGroupsSettings GetEditorGroupsSettings();
    }
}