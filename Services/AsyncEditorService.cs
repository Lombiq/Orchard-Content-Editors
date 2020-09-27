using Lombiq.ContentEditors.Authorization;
using Lombiq.ContentEditors.Constants;
using Lombiq.ContentEditors.Models;
using Orchard.Mvc;
using Orchard.Security;
using Orchard.Security.Permissions;
using System;
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
        private readonly IHttpContextAccessor _hca;


        public AsyncEditorService(IAuthorizer authorizer, IHttpContextAccessor hca)
        {
            _authorizer = authorizer;
            _hca = hca;
        }


        public bool IsAuthorized(AsyncEditorPart part, Permission permission, string group = null)
        {
            if (!part.HasEditorGroups || string.IsNullOrEmpty(group))
                return _authorizer.Authorize(permission, part);

            var originalEditorGroup = part.CurrentEditorGroup;
            part.SetCurrentEditorGroup(group);

            DynamicGroupPermissions.GroupPermissionTemplates.TryGetValue(
                permission.Name,
                out var dynamicGroupPermissionTemplate);

            if (dynamicGroupPermissionTemplate == null) return false;

            var dynamicGroupPermission = DynamicGroupPermissions.CreateDynamicPermission(
                dynamicGroupPermissionTemplate,
                part.TypeDefinition,
                group);

            if (dynamicGroupPermission == null) return false;

            part.CurrentEditorGroup = originalEditorGroup;

            return _authorizer.Authorize(dynamicGroupPermission, part);
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