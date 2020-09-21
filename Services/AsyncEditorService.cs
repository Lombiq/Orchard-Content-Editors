using Lombiq.ContentEditors.Authorization;
using Lombiq.ContentEditors.Constants;
using Lombiq.ContentEditors.Models;
using Orchard.ContentManagement;
using Orchard.Mvc;
using Orchard.Security;
using Orchard.Security.Permissions;
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
        private readonly IHttpContextAccessor _hca;


        public AsyncEditorService(
            IAuthorizer authorizer,
            IContentManager contentManager,
            IHttpContextAccessor hca)
        {
            _authorizer = authorizer;
            _contentManager = contentManager;
            _hca = hca;
        }


        public dynamic BuildAsyncEditorShape(AsyncEditorPart part, string group = "", dynamic shape = null)
        {
            part.SetCurrentEditorGroup(group);
            part.IsAsyncEditorContext = true;

            return shape ?? _contentManager.BuildEditor(part, group);
        }

        public bool IsAuthorized(AsyncEditorPart part, Permission permission, string group = null)
        {
            if (!part.HasEditorGroups || string.IsNullOrEmpty(group))
                return _authorizer.Authorize(permission, part);

            var originalEditorGroup = part.CurrentEditorGroup;
            part.SetCurrentEditorGroup(group);

            DynamicGroupPermissions.GroupPermissionTemplates.TryGetValue(permission.Name, out var dynamicGroupPermissionTemplate);
            if (dynamicGroupPermissionTemplate == null) return false;

            var dynamicGroupPermission = DynamicGroupPermissions.CreateDynamicPermission(dynamicGroupPermissionTemplate, part.TypeDefinition, group);
            if (dynamicGroupPermission == null) return false;

            part.CurrentEditorGroup = originalEditorGroup;

            return _authorizer.Authorize(dynamicGroupPermission, part);
        }

        public IEnumerable<EditorGroupDescriptor> GetAvailableEditorGroups(AsyncEditorPart part, bool authorizedOnly = false)
        {
            var editorGroups = part.GetEditorGroupList(authorizedOnly);
            if (editorGroups == null) return Enumerable.Empty<EditorGroupDescriptor>();

            return editorGroups.Where(editorGroup => IsEditorGroupAvailable(part, editorGroup.Name));
        }

        public bool IsEditorGroupAvailable(AsyncEditorPart part, string group)
        {
            Argument.ThrowIfNullOrEmpty(group, nameof(group));

            var editorGroup = part.GetEditorGroupDescriptor(group);
            if (editorGroup == null) return false;

            if (part.GetCompletedEditorGroups().Contains(editorGroup)) return true;

            return editorGroup.Equals(part.GetIncompleteEditorGroups().FirstOrDefault());
        }

        public EditorGroupDescriptor GetNextGroupDescriptor(AsyncEditorPart part, string group, bool authorizedOnly = false)
        {
            Argument.ThrowIfNullOrEmpty(group, nameof(group));

            var editorGroups = part.GetEditorGroupList(authorizedOnly);
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

            var editorGroups = part.GetEditorGroupList(authorizedOnly);
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

            if (!part.EditorGroupsSettings?.EditorGroups.Any(editorGroup => editorGroup.Name == group) ?? false) return;

            part.CompletedEditorGroupNames = part.GetCompletedEditorGroups()
                .Select(editorGroup => editorGroup.Name)
                .Union(new[] { group });
        }

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
    }
}