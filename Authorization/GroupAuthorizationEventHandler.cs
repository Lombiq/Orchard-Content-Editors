using Lombiq.ContentEditors.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Core.Contents;
using Orchard.Security;
using Orchard.Security.Permissions;

namespace Lombiq.ContentEditors.Authorization
{
    /// <summary>
    /// Adjusts permissions to utilize editor group permissions. Mostly follows the built-in dynamic content permission
    /// event handler (<see cref="Orchard.Core.Contents.Security.AuthorizationEventHandler"/>), and sadly parts of it
    /// need to be copy-pasted. Note that for content editor groups group permissions need to be used, content
    /// permissions can't be (because group permissions will override the currently checked permission).
    /// </summary>
    public class GroupAuthorizationEventHandler : IAuthorizationServiceEventHandler
    {
        public void Adjust(CheckAccessContext context) { }
        public void Complete(CheckAccessContext context) { }

        // Note that this will only work if it's in Checking(), not Adjust() (despite it better fitting Adjust()). In 
        // that case if the content type is securable then the built-in event handler will swap out the currently checked 
        // permission with a dynamic one (like Edit_MyContentType), rendering this event handler unable to detect the 
        // proper permissions.
        public void Checking(CheckAccessContext context)
        {
            var asyncEditorPart = context.Content.As<AsyncEditorPart>();

            if (context.Granted ||
                !context.Content.Has<ICommonPart>() ||
                asyncEditorPart == null ||
                string.IsNullOrEmpty(asyncEditorPart.CurrentEditorGroup?.Name))
            {
                return;
            }

            var ownerVariation = GetOwnerVariation(context.Permission);
            if (ownerVariation != null && HasOwnership(context.User, context.Content))
            {
                context.Adjusted = true;
                context.Permission = ownerVariation;
            }

            var permission = DynamicGroupPermissions.ConvertToDynamicPermissionTemplate(context.Permission);

            if (permission != null)
            {
                context.Adjusted = true;
                context.Permission = DynamicGroupPermissions.CreateDynamicPermission(
                    permission,
                    context.Content.ContentItem.TypeDefinition,
                    asyncEditorPart.CurrentEditorGroup.Name);
            }
        }

        private static bool HasOwnership(IUser user, IContent content)
        {
            if (user == null || content == null) return false;

            var commonPart = content.As<ICommonPart>();
            if (commonPart == null || commonPart.Owner == null) return false;

            return user.Id == commonPart.Owner.Id;
        }

        private static Permission GetOwnerVariation(Permission permission)
        {
            if (permission.Name == Permissions.PublishContent.Name)
                return Permissions.PublishOwnContent;
            if (permission.Name == Permissions.EditContent.Name)
                return Permissions.EditOwnContent;
            if (permission.Name == Permissions.DeleteContent.Name)
                return Permissions.DeleteOwnContent;
            if (permission.Name == Permissions.ViewContent.Name)
                return Permissions.ViewOwnContent;
            if (permission.Name == Permissions.PreviewContent.Name)
                return Permissions.PreviewOwnContent;

            return null;
        }
    }
}