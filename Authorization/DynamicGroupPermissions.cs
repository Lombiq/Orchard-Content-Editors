using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Lombiq.ContentEditors.Services;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Core.Contents;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;

namespace Lombiq.ContentEditors.Authorization
{
    /// <summary>
    /// Permissions dynamically generated for editor groups.
    /// </summary>
    public class DynamicGroupPermissions : IPermissionProvider
    {
        private static readonly Permission PublishContent = new Permission
        {
            Description = "Publish or unpublish {0} on group {1} for others",
            Name = "Publish_{0}_{1}",
            ImpliedBy = new[] { Permissions.PublishContent }
        };

        private static readonly Permission PublishOwnContent = new Permission
        {
            Description = "Publish or unpublish {0} on group {1}",
            Name = "PublishOwn_{0}_{1}",
            ImpliedBy = new[] { PublishContent, Permissions.PublishOwnContent }
        };

        private static readonly Permission EditContent = new Permission
        {
            Description = "Edit {0} on group {1} for others",
            Name = "Edit_{0}_{1}",
            ImpliedBy = new[] { PublishContent, Permissions.EditContent }
        };

        private static readonly Permission EditOwnContent = new Permission
        {
            Description = "Edit {0} on group {1}",
            Name = "EditOwn_{0}_{1}",
            ImpliedBy = new[] { EditContent, PublishOwnContent, Permissions.EditOwnContent }
        };

        public static readonly Dictionary<string, Permission> PermissionTemplates = new Dictionary<string, Permission>
        {
            [Permissions.PublishContent.Name] = PublishContent,
            [Permissions.PublishOwnContent.Name] = PublishOwnContent,
            [Permissions.EditContent.Name] = EditContent,
            [Permissions.EditOwnContent.Name] = EditOwnContent
        };

        private readonly IContentDefinitionManager _contentDefinitionManager;
        private readonly IEnumerable<IEditorGroupsProvider> _editorGroupsProviders;

        public virtual Feature Feature { get; set; }


        public DynamicGroupPermissions(
            IContentDefinitionManager contentDefinitionManager, 
            IEnumerable<IEditorGroupsProvider> editorGroupsProviders)
        {
            _contentDefinitionManager = contentDefinitionManager;
            _editorGroupsProviders = editorGroupsProviders;
        }


        public IEnumerable<Permission> GetPermissions()
        {
            var typeDefinitions = _contentDefinitionManager.ListTypeDefinitions();

            foreach (var typeDefinition in typeDefinitions)
            {
                var editorGroups = _editorGroupsProviders
                    .Where(editorGroupsProvider => editorGroupsProvider.CanProvideEditorGroups(typeDefinition.Name))
                    .SelectMany(editorGroupsProvider => editorGroupsProvider.GetEditorGroupsSettings().EditorGroups);

                foreach (var editorGroup in editorGroups)
                {
                    foreach (var permissionTemplate in PermissionTemplates.Values)
                    {
                        yield return CreateDynamicPermission(permissionTemplate, typeDefinition, editorGroup.Name);
                    }
                }
            }
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes() => Enumerable.Empty<PermissionStereotype>();

        /// <summary>
        /// Creates a dynamic permission template for a content type, based on a global content permission template.
        /// </summary>
        public static Permission ConvertToDynamicPermissionTemplate(Permission permission) =>
            PermissionTemplates.ContainsKey(permission.Name) ? PermissionTemplates[permission.Name] : null;

        /// <summary>
        /// Generates a permission dynamically for a content type and editor group.
        /// </summary>
        public static Permission CreateDynamicPermission(
            Permission template, ContentTypeDefinition typeDefinition, string group) =>
            new Permission
            {
                Name = string.Format(template.Name, typeDefinition.Name, group),
                Description = string.Format(template.Description, typeDefinition.DisplayName, group),
                Category = typeDefinition.DisplayName,
                ImpliedBy = (template.ImpliedBy ?? new Permission[0]).Select(t => CreateDynamicPermission(t, typeDefinition, group))
            };
    }
}