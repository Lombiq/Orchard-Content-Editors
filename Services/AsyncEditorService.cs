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
        private readonly IEditorGroupsProviderAccessor _editorGroupsProviderAccessor;
        private readonly IJsonConverter _jsonConverter;


        public AsyncEditorService(IAuthorizer authorizer, IContentManager contentManager, IEditorGroupsProviderAccessor editorGroupsProviderAccessor, IJsonConverter jsonConverter)
        {
            _authorizer = authorizer;
            _contentManager = contentManager;
            _editorGroupsProviderAccessor = editorGroupsProviderAccessor;
            _jsonConverter = jsonConverter;
        }


        public dynamic BuildAsyncEditorShape(AsyncEditorPart part, string group = "")
        {
            SetCurrentGroup(part, group);
            SetAsyncEditorContext(part);

            return _contentManager.BuildEditor(part, group);
        }

        public bool AuthorizeToEdit(AsyncEditorPart part, string group = "")
        {
            // Commented out temporary.
            //SetCurrentGroup(part, group);

            return _authorizer.Authorize(Permissions.EditContent, part);
        }

        public bool AuthorizeToPublish(AsyncEditorPart part, string group = "")
        {
            // Commented out temporary.
            //SetCurrentGroup(part, group);

            return _authorizer.Authorize(Permissions.PublishContent, part);
        }

        public IEnumerable<EditorGroupDescriptor> GetAuthorizedEditorGroups(AsyncEditorPart part)
        {
            var editorGroups = GetEditorGroupsSettings(part)?.EditorGroups;
            if (editorGroups == null) return Enumerable.Empty<EditorGroupDescriptor>();

            var authorizedEditorGroups = new List<EditorGroupDescriptor>();
            foreach (var editorGroup in editorGroups)
            {
                if (AuthorizeToEdit(part, editorGroup.Name))
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

        public IEnumerable<EditorGroupDescriptor> GetCompleteEditorGroups(AsyncEditorPart part, bool authorizedOnly = false)
        {
            var editorGroups = GetEditorGroupList(part, authorizedOnly);
            if (editorGroups == null) return Enumerable.Empty<EditorGroupDescriptor>();

            var completeGroupNames = !string.IsNullOrEmpty(part.CompleteEditorGroupNamesSerialized) ?
                _jsonConverter.Deserialize<IEnumerable<string>>(part.CompleteEditorGroupNamesSerialized)
                : Enumerable.Empty<string>();

            return completeGroupNames
                .Select(groupName => editorGroups.FirstOrDefault(group => group.Name == groupName))
                .Where(group => group != null);
        }

        public IEnumerable<EditorGroupDescriptor> GetIncompleteEditorGroups(AsyncEditorPart part, bool authorizedOnly = false)
        {
            var editorGroups = GetEditorGroupList(part, authorizedOnly);
            if (editorGroups == null) return Enumerable.Empty<EditorGroupDescriptor>();

            return editorGroups.Except(GetCompleteEditorGroups(part));
        }

        public IEnumerable<EditorGroupDescriptor> GetAvailableEditorGroups(AsyncEditorPart part, bool authorizedOnly = false)
        {
            var editorGroups = GetEditorGroupList(part, authorizedOnly);
            if (editorGroups == null) return Enumerable.Empty<EditorGroupDescriptor>();

            return editorGroups.Where(editorGroup => EditorGroupAvailable(part, editorGroup.Name));
        }

        public EditorGroupDescriptor GetEditorGroupDescriptor(AsyncEditorPart part, string group) =>
            GetEditorGroupsSettings(part)?.EditorGroups.FirstOrDefault(editorGroup => editorGroup.Name == group);

        public bool EditorGroupAvailable(AsyncEditorPart part, string group)
        {
            Argument.ThrowIfNullOrEmpty(group, nameof(group));

            var editorGroup = GetEditorGroupDescriptor(part, group);
            if (editorGroup == null) return false;

            if (GetCompleteEditorGroups(part).Contains(editorGroup)) return true;

            return editorGroup.Equals(GetIncompleteEditorGroups(part).FirstOrDefault());
        }

        public EditorGroupDescriptor GetNextGroupDescriptor(AsyncEditorPart part, string group, bool authorizedOnly = false)
        {
            Argument.ThrowIfNullOrEmpty(group, nameof(group));

            var editorGroups = GetEditorGroupList(part, authorizedOnly);
            if (!editorGroups?.Any() ?? false) return null;

            var groupDescriptor = editorGroups.FirstOrDefault(editorGroup => editorGroup.Name == group);
            if (groupDescriptor == null) return null;

            return groupDescriptor.Equals(editorGroups.Last()) ? 
                null : 
                editorGroups[editorGroups.IndexOf(groupDescriptor) + 1];
        }

        public EditorGroupDescriptor GetPreviousGroupDescriptor(AsyncEditorPart part, string group, bool authorizedOnly = false)
        {
            Argument.ThrowIfNullOrEmpty(group, nameof(group));

            var editorGroups = GetEditorGroupList(part, authorizedOnly);
            if (!editorGroups?.Any() ?? false) return null;

            var groupDescriptor = editorGroups.FirstOrDefault(editorGroup => editorGroup.Name == group);
            if (groupDescriptor == null) return null;

            return groupDescriptor == editorGroups.First() ? 
                null :
                editorGroups[editorGroups.IndexOf(groupDescriptor) - 1];
        }

        public void StoreCompleteEditorGroup(AsyncEditorPart part, string group)
        {
            Argument.ThrowIfNullOrEmpty(group, nameof(group));

            if (!GetEditorGroupsSettings(part)?.EditorGroups.Any(editorGroup => editorGroup.Name == group) ?? false) return;

            var completeGroupNames = GetCompleteEditorGroups(part).Select(editorGroup => editorGroup.Name).Union(new[] { group });

            part.CompleteEditorGroupNamesSerialized = _jsonConverter.Serialize(completeGroupNames);
        }

        public EditorGroupsSettings GetEditorGroupsSettings(AsyncEditorPart part) =>
            _editorGroupsProviderAccessor.GetProvider(part.ContentItem.ContentType)?.GetEditorGroupsSettings();


        private void SetCurrentGroup(AsyncEditorPart part, string group) =>
            part.CurrentEditorGroup = GetEditorGroupDescriptor(part, group);

        private void SetAsyncEditorContext(AsyncEditorPart part) =>
            part.AsyncEditorContext = true;

        private IList<EditorGroupDescriptor> GetEditorGroupList(AsyncEditorPart part, bool authorizedOnly) =>
            authorizedOnly ?
                GetAuthorizedEditorGroups(part).ToList() :
                GetEditorGroupsSettings(part)?.EditorGroups.ToList();
    }
}