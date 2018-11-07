using Lombiq.ContentEditors.Constants;
using Lombiq.ContentEditors.Models;
using Lombiq.ContentEditors.Services;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Aspects;
using Orchard.Core.Contents;
using Orchard.Logging;
using Orchard.Mvc;
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
        private readonly IAsyncEditorService _asyncEditorService;
        private readonly IHttpContextAccessor _hca;
        

        public GroupAuthorizationEventHandler(IAsyncEditorService asyncEditorService, IHttpContextAccessor hca)
        {
            _asyncEditorService = asyncEditorService;
            _hca = hca;
        }


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
            if (ownerVariation != null && HasOwnership(context.User, asyncEditorPart))
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

        private bool HasOwnership(IUser user, AsyncEditorPart asyncEditorPart)
        {
            if (user == null)
            {
                // If the content is new then the editor cookie won't identify the content editing session
                // since the ID is 0. Use ownership if that is the case.
                if (asyncEditorPart.IsNew() || asyncEditorPart.IsContentCreationFailed ||
                    _asyncEditorService.ValidateEditorSessionCookie(asyncEditorPart)) return true;

                return false;
            }

            var commonPart = asyncEditorPart.As<ICommonPart>();
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