using Lombiq.EditorGroups.Models;
using Orchard;
using System.Collections.Generic;

namespace Lombiq.EditorGroups.Services
{
    public interface IAsyncEditorService : IDependency
    {
        bool AuthorizeToEdit(AsyncEditorPart part, string group = "");
        bool AuthorizeToPublish(AsyncEditorPart part, string group = "");
        IEnumerable<EditorGroupDescriptor> GetCompleteEditorGroups(AsyncEditorPart part, bool authorizedOnly = false);
        IEnumerable<EditorGroupDescriptor> GetIncompleteEditorGroups(AsyncEditorPart part, bool authorizedOnly = false);
        IEnumerable<EditorGroupDescriptor> GetAvailableEditorGroups(AsyncEditorPart part, bool authorizedOnly = false);
        IEnumerable<EditorGroupDescriptor> GetAuthorizedEditorGroups(AsyncEditorPart part);
        dynamic BuildAsyncEditorShape(AsyncEditorPart part, string group = "");
        EditorGroupDescriptor GetEditorGroupDescriptor(AsyncEditorPart part, string group);
        EditorGroupDescriptor GetNextGroupDescriptor(AsyncEditorPart part, string group, bool authorizedOnly = false);
        EditorGroupDescriptor GetPreviousGroupDescriptor(AsyncEditorPart part, string group, bool authorizedOnly = false);
        bool EditorGroupAvailable(AsyncEditorPart part, string group);
        void StoreCompleteEditorGroup(AsyncEditorPart part, string group);
        EditorGroupsSettings GetEditorGroupsSettings(AsyncEditorPart part);
    }
}