using Lombiq.EditorGroups.Models;
using Orchard;
using Orchard.ContentManagement;

namespace Lombiq.EditorGroups.Services
{
    public interface IAsyncEditorService : IDependency
    {
        bool IsAuthorizedToEditGroup(EditorGroupsPart part, string group);
        dynamic GetAsyncEditorShape(EditorGroupsPart part, string group);
        EditorGroupDescriptor GetEditorGroupDescriptor(EditorGroupsPart part, string group);
    }
}