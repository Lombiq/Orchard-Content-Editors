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
        private readonly IEditorGroupsProviderAccessor _editorGroupsProviderAccessor;


        public AsyncEditorService(IAuthorizer authorizer, IContentManager contentManager, IEditorGroupsProviderAccessor editorGroupsProviderAccessor)
        {
            _authorizer = authorizer;
            _contentManager = contentManager;
            _editorGroupsProviderAccessor = editorGroupsProviderAccessor;
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

            // Commented out temporary.
            //SetCurrentGroup(part, group);

            return _authorizer.Authorize(Permissions.EditContent, part);
        }

        public IEnumerable<EditorGroupDescriptor> GetAuthorizedGroups(EditorGroupsPart part)
        {
            var authorizedEditorGroups = new List<EditorGroupDescriptor>();
            foreach (var editorGroup in GetEditorGroupsSettings(part).EditorGroups)
            {
                if (IsAuthorizedToEditGroup(part, editorGroup.Name))
                {
                    authorizedEditorGroups.Add(editorGroup);

                    continue;
                }

                if (GetEditorGroupsSettings(part).UnauthorizedEditorGroupBehavior == 
                    UnauthorizedEditorGroupBehavior.AllowEditingUntilFirstUnauthorizedGroup)
                {
                    break;
                }
            }

            return authorizedEditorGroups;
        }

        public EditorGroupDescriptor GetEditorGroupDescriptor(EditorGroupsPart part, string group) =>
            GetEditorGroupsSettings(part).EditorGroups.FirstOrDefault(editorGroup => editorGroup.Name == group);

        public bool EditorGroupAvailable(EditorGroupsPart part, string group)
        {
            if (!GetEditorGroupsSettings(part).EditorGroups.Any(editorGroup => editorGroup.Name == group)) return false;

            if (part.CompleteEditorGroupNames.Contains(group)) return true;

            return group == part.IncompleteEditorGroupNames.FirstOrDefault();
        }

        public EditorGroupDescriptor GetNextAuthorizedGroupDescriptor(EditorGroupsPart part, string group)
        {
            var authorizedGroups = GetAuthorizedGroups(part).ToList();

            var groupDescriptor = authorizedGroups.FirstOrDefault(editorGroup => editorGroup.Name == group);
            if (groupDescriptor == null) return null;

            return groupDescriptor == authorizedGroups.Last() ? 
                null : 
                authorizedGroups[authorizedGroups.IndexOf(groupDescriptor) + 1];
        }

        public EditorGroupDescriptor GetPreviousAuthorizedGroupDescriptor(EditorGroupsPart part, string group)
        {
            var authorizedGroups = GetAuthorizedGroups(part).ToList();

            var groupDescriptor = authorizedGroups.FirstOrDefault(editorGroup => editorGroup.Name == group);
            if (groupDescriptor == null) return null;

            return groupDescriptor == authorizedGroups.First() ? 
                null : 
                authorizedGroups[authorizedGroups.IndexOf(groupDescriptor) - 1];
        }

        public void StoreCompleteEditorGroup(EditorGroupsPart part, string group)
        {
            if (!GetEditorGroupsSettings(part).EditorGroups.Any(editorGroup => editorGroup.Name == group)) return;

            var completeGroups = part.CompleteEditorGroupNames.Union(new[] { group });

            part.CompleteEditorGroupNames = completeGroups;
        }

        public EditorGroupsSettings GetEditorGroupsSettings(EditorGroupsPart part) =>
            _editorGroupsProviderAccessor.GetProvider(part.ContentItem.ContentType)?.GetEditorGroupsSettings();


        private void SetCurrentGroup(EditorGroupsPart part, string group) =>
            part.CurrentEditorGroup = GetEditorGroupDescriptor(part, group);

        private void SetAsyncEditorContext(EditorGroupsPart part) =>
            part.AsyncEditorContext = true;
    }
}