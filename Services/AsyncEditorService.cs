using Lombiq.EditorGroups.Models;
using Orchard.ContentManagement;
using Orchard.Core.Contents;
using Orchard.Security;
using Orchard.Services;
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

        public EditorGroupDescriptor GetEditorGroupDescriptor(EditorGroupsPart part, string group) =>
            part.EditorGroups.FirstOrDefault(editorGroup => editorGroup.Name == group);

        public bool EditorGroupAvailable(EditorGroupsPart part, string group)
        {
            if (!part.EditorGroups.Any(editorGroup => editorGroup.Name == group)) return false;

            if (part.CompleteEditorGroupNames.Contains(group)) return true;

            return group == part.IncompleteEditorGroupNames.FirstOrDefault();
        }

        public EditorGroupDescriptor GetNextEditorGroupDescriptor(EditorGroupsPart part, string group = "")
        {
            var authorizedGroups = part.AuthorizedEditorGroups.ToList();

            if (string.IsNullOrEmpty(group))
            {
                group = part.CompleteEditorGroupNames.LastOrDefault();

                if (string.IsNullOrEmpty(group))
                {
                    return part.AuthorizedEditorGroups.FirstOrDefault();
                }
            }

            var lastCompleteGroupDescriptor = authorizedGroups
                .FirstOrDefault(groupDescriptor => groupDescriptor.Name == group);

            if (lastCompleteGroupDescriptor == null) return null;

            var indexOfLastCompleteGroup = authorizedGroups.IndexOf(lastCompleteGroupDescriptor);

            if (indexOfLastCompleteGroup == authorizedGroups.Count - 1) return null;

            return authorizedGroups[indexOfLastCompleteGroup + 1];
        }

        public void StoreCompleteEditorGroup(EditorGroupsPart part, string group)
        {
            if (!part.EditorGroups.Any(editorGroup => editorGroup.Name == group)) return;

            var completeGroups = part.CompleteEditorGroupNames.Union(new[] { group });

            part.CompleteEditorGroupNames = completeGroups;
        }


        private void SetCurrentGroup(EditorGroupsPart part, string group) =>
            part.CurrentEditorGroup = GetEditorGroupDescriptor(part, group);

        private void SetAsyncEditorContext(EditorGroupsPart part) =>
            part.AsyncEditorContext = true;
    }
}