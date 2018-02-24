using Lombiq.EditorGroups.Models;
using Orchard;
using System.Collections.Generic;

namespace Lombiq.EditorGroups.Services
{
    public interface IAsyncEditorService : IDependency
    {
        bool IsAuthorizedToEditGroup(AsyncEditorPart part, string group);
        IEnumerable<EditorGroupDescriptor> GetAuthorizedEditorGroups(AsyncEditorPart part);
        dynamic BuildAsyncEditorShape(AsyncEditorPart part, string group);
        EditorGroupDescriptor GetEditorGroupDescriptor(AsyncEditorPart part, string group);
        EditorGroupDescriptor GetNextAuthorizedGroupDescriptor(AsyncEditorPart part, string group = "");
        EditorGroupDescriptor GetPreviousAuthorizedGroupDescriptor(AsyncEditorPart part, string group = "");
        bool EditorGroupAvailable(AsyncEditorPart part, string group);
        void StoreCompleteEditorGroup(AsyncEditorPart part, string group);
        EditorGroupsSettings GetEditorGroupsSettings(AsyncEditorPart part);
    }
}