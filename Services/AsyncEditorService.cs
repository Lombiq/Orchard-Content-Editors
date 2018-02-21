using Lombiq.EditorGroups.Models;
using Orchard.ContentManagement;
using Orchard.Core.Contents;
using Orchard.Security;
using Orchard.Validation;
using System.Collections.Generic;
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


        public dynamic BuildAsyncEditorShape(EditorGroupsPart part, string group)
        {
            Argument.ThrowIfNullOrEmpty(group, nameof(group));

            SetCurrentGroup(part, group);
            SetAsyncEditorContext(part);

            return _contentManager.BuildEditor(part, group);
        }

        public bool IsAuthorizedToEditGroup(EditorGroupsPart part, string group)
        {
            Argument.ThrowIfNullOrEmpty(group, nameof(group));

            //SetCurrentGroup(part, group);

            return _authorizer.Authorize(Permissions.EditContent, part);
        }

        public IEnumerable<EditorGroupDescriptor> GetAuthorizedGroups(EditorGroupsPart part)
        {
            var authorizedEditorGroups = new List<EditorGroupDescriptor>();
            foreach (var editorGroup in part.EditorGroups)
            {
                if (IsAuthorizedToEditGroup(part, editorGroup.Name))
                {
                    authorizedEditorGroups.Add(editorGroup);

                    continue;
                }

                if (part.UnauthorizedEditorGroupBehavior == UnauthorizedEditorGroupBehavior.AllowEditingUntilFirstUnauthorizedGroup)
                {
                    break;
                }
            }

            return authorizedEditorGroups;
        }

        public EditorGroupDescriptor GetEditorGroupDescriptor(EditorGroupsPart part, string group)
        {
            var currentGroup = part.EditorGroups.FirstOrDefault(editorGroup => editorGroup.Name == group);

            return currentGroup;
        }

        public bool EditorGroupAvailable(EditorGroupsPart part, string group)
        {
            if (!part.EditorGroups.Any(editorGroup => editorGroup.Name == group)) return false;

            if (part.CompleteEditorGroupNames.Contains(group)) return true;

            return group == part.IncompleteEditorGroupNames.FirstOrDefault();
        }


        private void SetCurrentGroup(EditorGroupsPart part, string group) =>
            part.CurrentEditorGroup = GetEditorGroupDescriptor(part, group);

        private void SetAsyncEditorContext(EditorGroupsPart part) =>
            part.AsyncEditorContext = true;
    }
}