using Lombiq.EditorGroups.Models;
using Orchard.ContentManagement;
using Orchard.Core.Contents;
using Orchard.Security;
using Orchard.Validation;
using System.Linq;

namespace Lombiq.EditorGroups.Services
{
    public class AsyncEditorService : IAsyncEditorService
    {
        private readonly IAuthorizer _authorizer;
        private readonly IContentManager _contentManager;


        public AsyncEditorService(IAuthorizer authorizer, IContentManager contentManager)
        {
            _authorizer = authorizer;
            _contentManager = contentManager;
        }


        public dynamic GetAsyncEditorShape(EditorGroupsPart part, string group)
        {
            Argument.ThrowIfNullOrEmpty(group, nameof(group));

            SetCurrentGroup(part, group);
            SetAsyncEditorContext(part);

            return _contentManager.BuildEditor(part, group);
        }

        public bool IsAuthorizedToEditGroup(EditorGroupsPart part, string group)
        {
            Argument.ThrowIfNullOrEmpty(group, nameof(group));

            SetCurrentGroup(part, group);

            return _authorizer.Authorize(Permissions.EditContent, part);
        }

        public EditorGroupDescriptor GetEditorGroupDescriptor(EditorGroupsPart part, string group) =>
            part.EditorGroups.FirstOrDefault(editorGroup => editorGroup.Name == group);


        private void SetCurrentGroup(EditorGroupsPart part, string group) =>
            part.CurrentEditorGroup = GetEditorGroupDescriptor(part, group);

        private void SetAsyncEditorContext(EditorGroupsPart part) =>
            part.AsyncEditorContext = true;
    }
}