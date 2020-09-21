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
        /// Initializes the given content item to be edited async and also returns the generated editor shape.
        /// </summary>
        /// <param name="part">AsyncEditorPart of the content item.</param>
        /// <param name="group">Optionally the editor group that the editor shape is generated for.</param>
        /// <returns>Editor shape for the given editor group.</returns>
        dynamic BuildAsyncEditorShape(AsyncEditorPart part, string group = "", dynamic shape = null);

        /// <summary>
        /// Registers the given editor group as complete.
        /// </summary>
        /// <param name="part">AsyncEditorPart of the content item.</param>
        /// <param name="group">Name of the group that needs to be stored.</param>
        void StoreCompletedEditorGroup(AsyncEditorPart part, string group);

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