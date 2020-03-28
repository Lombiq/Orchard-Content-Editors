using Orchard.Security;
using Orchard.Security.Permissions;
using System;
using System.Linq;
using CorePermissions = Orchard.Core.Contents.Permissions;

namespace Lombiq.ContentEditors.Authorization
{
    /// <summary>
    /// Check group-specific permission and if granted, adjust to the corresponding "action" (Publish or Edit) permission.
    /// Basically it will authorize the User to edit or publish a given group if they can edit or publish that content item
    /// and they have the group-specific permission too.
    /// </summary>
    public class GroupAuthorizationEventHandler : IAuthorizationServiceEventHandler
    {
        private readonly IAuthorizer _authorizer;


        public GroupAuthorizationEventHandler(IAuthorizer authorizer)
        {
            _authorizer = authorizer;
        }


        public void Complete(CheckAccessContext context)
        {
            if (!context.Granted || context.User == null || context.Content == null || context.Permission == null) return;

            if (!context.Permission.Name.Contains(DynamicGroupPermissions.DynamicGroupPermissionNameMarker)) return;

            Permission corePermission = null;
            var permissionAction = context.Permission.Name.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries).FirstOrDefault() ?? "";
            switch (permissionAction)
            {
                case "Edit":
                    corePermission = CorePermissions.EditContent;

                    break;
                case "Publish":
                    corePermission = CorePermissions.PublishContent;

                    break;
                default:
                    break;
            }

            if (corePermission == null) context.Granted = false;
            else context.Granted = _authorizer.Authorize(corePermission, context.Content);
        }

        public void Adjust(CheckAccessContext _) { }

        public void Checking(CheckAccessContext _) { }
    }
}