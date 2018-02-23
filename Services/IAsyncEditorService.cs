using Lombiq.EditorGroups.Models;
using Orchard;
using Orchard.ContentManagement;
using System.Collections.Generic;

namespace Lombiq.EditorGroups.Services
{
    public interface IAsyncEditorService : IDependency
    {
        bool IsAuthorizedToEditGroup(EditorGroupsPart part, string group);
        IEnumerable<EditorGroupDescriptor> GetAuthorizedGroups(EditorGroupsPart part);
        dynamic BuildAsyncEditorShape(EditorGroupsPart part, string group);
        EditorGroupDescriptor GetEditorGroupDescriptor(EditorGroupsPart part, string group);
        EditorGroupDescriptor GetNextEditorGroupDescriptor(EditorGroupsPart part, string group = "");
        bool EditorGroupAvailable(EditorGroupsPart part, string group);
        void StoreCompleteEditorGroup(EditorGroupsPart part, string group);
    }
}