using Lombiq.ContentEditors.Models;
using Orchard;
using Orchard.Core.Contents;
using Orchard.Security.Permissions;

namespace Lombiq.ContentEditors.Services
{
    public interface IAsyncEditorService : IDependency
    {
        /// <summary>
        /// Checks the current User is authorized with the specified permission
        /// on a given editor group of a content.
        /// </summary>
        /// <param name="part">AsyncEditorPart of the content item.</param>
        /// <param name="permission">Permission to check.</param>
        /// <param name="group">Editor group to be authorized on.</param>
        /// <returns>True if the current user is authorized.</returns>
        bool IsAuthorized(AsyncEditorPart part, Permission permission, string group = "");

        /// <summary>
        /// Validates cookie that identifies the editor session.
        /// </summary>
        /// <param name="part">AsyncEditorPart of the content item.</param>
        /// <returns>Returns true if the session is valid.</returns>
        bool ValidateEditorSessionCookie(AsyncEditorPart part);

        /// <summary>
        /// Creates cookie for identifying the current editor session.
        /// </summary>
        /// <param name="part">AsyncEditorPart of the content item.</param>
        void SetEditorSessionCookie(AsyncEditorPart part);

        /// <summary>
        /// Removes cookie that identifies the current editor session.
        /// </summary>
        /// <param name="part">AsyncEditorPart of the content item.</param>
        void RemoveEditorSessionCookie();
    }


    public static class AsyncEditorServiceExtensions
    {
        public static bool IsAuthorizedToEdit(
            this IAsyncEditorService service,
            AsyncEditorPart part,
            string group = "") =>
            service.IsAuthorized(part, Permissions.EditContent, group);

        public static bool IsAuthorizedToPublish(
            this IAsyncEditorService service,
            AsyncEditorPart part,
            string group = "") =>
            service.IsAuthorized(part, Permissions.PublishContent, group);
    }
}