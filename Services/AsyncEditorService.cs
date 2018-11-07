using Lombiq.ContentEditors.Constants;
using Lombiq.ContentEditors.Models;
using Orchard.ContentManagement;
using Orchard.Core.Contents;
using Orchard.Mvc;
using Orchard.Security;
using Orchard.Security.Permissions;
using Orchard.Services;
using Orchard.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Helpers;

namespace Lombiq.ContentEditors.Services
{
    public class AsyncEditorService : IAsyncEditorService
    {
        private static readonly DateTime PastDateForExpiration = new DateTime(1990, 1, 1);

        private readonly IAuthorizer _authorizer;
        private readonly IContentManager _contentManager;
        private readonly IEditorGroupsProviderAccessor _editorGroupsProviderAccessor;
        private readonly IJsonConverter _jsonConverter;
        private readonly IHttpContextAccessor _hca;


        public AsyncEditorService(
            IAuthorizer authorizer,
            IContentManager contentManager,
            IEditorGroupsProviderAccessor editorGroupsProviderAccessor,
            IJsonConverter jsonConverter,
            IHttpContextAccessor hca)
        {
            _authorizer = authorizer;
            _contentManager = contentManager;
            _editorGroupsProviderAccessor = editorGroupsProviderAccessor;
            _jsonConverter = jsonConverter;
            _hca = hca;
        }


        public dynamic BuildAsyncEditorShape(AsyncEditorPart part, string group = "")
        {
            SetCurrentGroup(part, group);
            SetAsyncEditorContext(part);

            return _contentManager.BuildEditor(part, group);
        }

        public bool IsAuthorizedToEdit(AsyncEditorPart part, string group = "") =>
            IsAuthorized(part, group, Permissions.EditContent);

        public bool IsAuthorizedToPublish(AsyncEditorPart part, string group = "") =>
            IsAuthorized(part, group, Permissions.PublishContent);

        public IEnumerable<EditorGroupDescriptor> GetAuthorizedEditorGroups(AsyncEditorPart part)
        {
            var editorGroups = GetEditorGroupsSettings(part)?.EditorGroups;
            if (editorGroups == null) return Enumerable.Empty<EditorGroupDescriptor>();

            var authorizedEditorGroups = new List<EditorGroupDescriptor>();
            foreach (var editorGroup in editorGroups)
            {
                if (IsAuthorizedToEdit(part, editorGroup.Name))
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

        public IEnumerable<EditorGroupDescriptor> GetCompletedEditorGroups(AsyncEditorPart part, bool authorizedOnly = false)
        {
            var editorGroups = GetEditorGroupList(part, authorizedOnly);
            if (editorGroups == null) return Enumerable.Empty<EditorGroupDescriptor>();

            var completeGroupNames = !string.IsNullOrEmpty(part.CompletedEditorGroupNamesSerialized) ?
                _jsonConverter.Deserialize<IEnumerable<string>>(part.CompletedEditorGroupNamesSerialized) :
                Enumerable.Empty<string>();

            return completeGroupNames
                .Select(groupName => editorGroups.FirstOrDefault(group => group.Name == groupName))
                .Where(group => group != null);
        }

        public IEnumerable<EditorGroupDescriptor> GetIncompleteEditorGroups(AsyncEditorPart part, bool authorizedOnly = false)
        {
            var editorGroups = GetEditorGroupList(part, authorizedOnly);
            if (editorGroups == null) return Enumerable.Empty<EditorGroupDescriptor>();

            return editorGroups.Except(GetCompletedEditorGroups(part));
        }

        public IEnumerable<EditorGroupDescriptor> GetAvailableEditorGroups(AsyncEditorPart part, bool authorizedOnly = false)
        {
            var editorGroups = GetEditorGroupList(part, authorizedOnly);
            if (editorGroups == null) return Enumerable.Empty<EditorGroupDescriptor>();

            return editorGroups.Where(editorGroup => IsEditorGroupAvailable(part, editorGroup.Name));
        }

        public EditorGroupDescriptor GetEditorGroupDescriptor(AsyncEditorPart part, string group) =>
            GetEditorGroupsSettings(part)?.EditorGroups.FirstOrDefault(editorGroup => editorGroup.Name == group);

        public bool IsEditorGroupAvailable(AsyncEditorPart part, string group)
        {
            Argument.ThrowIfNullOrEmpty(group, nameof(group));

            var editorGroup = GetEditorGroupDescriptor(part, group);
            if (editorGroup == null) return false;

            if (GetCompletedEditorGroups(part).Contains(editorGroup)) return true;

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

        public void StoreCompletedEditorGroup(AsyncEditorPart part, string group)
        {
            Argument.ThrowIfNullOrEmpty(group, nameof(group));

            if (!GetEditorGroupsSettings(part)?.EditorGroups.Any(editorGroup => editorGroup.Name == group) ?? false) return;

            var completeGroupNames = GetCompletedEditorGroups(part).Select(editorGroup => editorGroup.Name).Union(new[] { group });

            part.CompletedEditorGroupNamesSerialized = _jsonConverter.Serialize(completeGroupNames);
        }

        public EditorGroupsSettings GetEditorGroupsSettings(AsyncEditorPart part) =>
            _editorGroupsProviderAccessor.GetProvider(part.ContentItem.ContentType)?.GetEditorGroupsSettings();

        public bool ValidateEditorSessionCookie(AsyncEditorPart part)
        {
            var editorSessionCookie = GetEditorSessionCookieFromRequest();
            if (editorSessionCookie == null) return false;

            return Crypto.VerifyHashedPassword(
                editorSessionCookie.Value,
                Encoding.Unicode.GetString(CombineIdAndEditorSalt(part)));
        }

        public void SetEditorSessionCookie(AsyncEditorPart part) =>
            // Hashing the content item ID using the cryptographically secure content item-specific salt
            // which can be used to identify editor session - typically for anonymous users who have permission
            // to edit the content item.
            _hca.Current().Response.SetCookie(new HttpCookie(
                CookieNames.CurrentEditorSession,
                Crypto.HashPassword(Encoding.Unicode.GetString(CombineIdAndEditorSalt(part))))
                {
                    HttpOnly = true
                });

        public void RemoveEditorSessionCookie()
        {
            if (GetEditorSessionCookieFromRequest() != null)
            {
                // A cookie with an already expired date will make the browser to remove the existing cookie.
                var expiringEditorSessionCookie = new HttpCookie(CookieNames.CurrentEditorSession)
                {
                    Expires = PastDateForExpiration,
                    HttpOnly = true
                };

                _hca.Current().Response.SetCookie(expiringEditorSessionCookie);
            }
        }


        private HttpCookie GetEditorSessionCookieFromRequest() =>
            _hca.Current().Request.Cookies[CookieNames.CurrentEditorSession];

        private byte[] CombineIdAndEditorSalt(AsyncEditorPart part) =>
            Convert.FromBase64String(part.EditorSessionSalt)
                .Concat(Encoding.Unicode.GetBytes(part.Id.ToString()))
                .ToArray();

        private void SetCurrentGroup(AsyncEditorPart part, string group) =>
            part.CurrentEditorGroup = GetEditorGroupDescriptor(part, group);

        private void SetAsyncEditorContext(AsyncEditorPart part) =>
            part.IsAsyncEditorContext = true;

        private IList<EditorGroupDescriptor> GetEditorGroupList(AsyncEditorPart part, bool authorizedOnly) =>
            authorizedOnly ?
                GetAuthorizedEditorGroups(part).ToList() :
                GetEditorGroupsSettings(part)?.EditorGroups.ToList();

        private bool IsAuthorized(AsyncEditorPart part, string group, Permission permission)
        {
            var originalEditorGroup = part.CurrentEditorGroup;
            SetCurrentGroup(part, group);

            var isAuthorized = _authorizer.Authorize(permission, part);

            part.CurrentEditorGroup = originalEditorGroup;
            return isAuthorized;
        }
    }
}