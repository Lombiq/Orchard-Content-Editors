using Lombiq.ContentEditors.Services;
using Orchard.ContentManagement.MetaData;
using Orchard.ContentManagement.MetaData.Models;
using Orchard.Core.Contents;
using Orchard.Environment.Extensions.Models;
using Orchard.Security.Permissions;
using System.Collections.Generic;
using System.Linq;

namespace Lombiq.ContentEditors.Authorization
{
    /// <summary>
    /// Permissions dynamically generated for editor groups.
    /// </summary>
    public class DynamicGroupPermissions : IPermissionProvider
    {
        public static string DynamicGroupPermissionNameMarker = "_Group";

        private static readonly Permission PublishAllContentGroup = new Permission
        {
            Description = "Publish or unpublish all {0} editor groups (if authorized to publish)",
            Name = $"Publish_{{0}}_{DynamicGroupPermissionNameMarker}-All",
            // DynamicPermissions doesn't expose the permission templates, but we can get the appropriate one
            // from the name of the core permission it's based on.
            // This way these templates can be "implied by" the type-specific dynamic permission.
            ImpliedBy = new[] { DynamicPermissions.PermissionTemplates[Permissions.PublishContent.Name] }
        };

        private static readonly Permission EditAllContentGroup = new Permission
        {
            Description = "Edit all {0} editor groups (if authorized to edit)",
            Name = $"Edit_{{0}}_{DynamicGroupPermissionNameMarker}-All",
            ImpliedBy = new[] { PublishAllContentGroup, DynamicPermissions.PermissionTemplates[Permissions.EditContent.Name] }
        };

        private static readonly Permission PublishContentGroup = new Permission
        {
            Description = "Publish or unpublish {0} group {1} (if authorized to publish)",
            Name = $"Publish_{{0}}_{DynamicGroupPermissionNameMarker}_{{1}}",
            ImpliedBy = new[] { PublishAllContentGroup }
        };

        private static readonly Permission EditContentGroup = new Permission
        {
            Description = "Edit {0} group {1} (if authorized to edit)",
            Name = $"Edit_{{0}}_{DynamicGroupPermissionNameMarker}_{{1}}",
            ImpliedBy = new[] { PublishContentGroup, EditAllContentGroup }
        };

        public static readonly Dictionary<string, Permission> GroupPermissionTemplates = new Dictionary<string, Permission>
        {
            [nameof(PublishAllContentGroup)] = PublishAllContentGroup,
            [nameof(EditAllContentGroup)] = EditAllContentGroup,
            [Permissions.PublishContent.Name] = PublishContentGroup,
            [Permissions.EditContent.Name] = EditContentGroup,
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

                if (editorGroups.Any())
                {
                    yield return CreateDynamicPermission(PublishAllContentGroup, typeDefinition, "");
                    yield return CreateDynamicPermission(EditAllContentGroup, typeDefinition, "");

                    foreach (var editorGroup in editorGroups)
                    {
                        yield return CreateDynamicPermission(PublishContentGroup, typeDefinition, editorGroup.Name);
                        yield return CreateDynamicPermission(EditContentGroup, typeDefinition, editorGroup.Name);
                    }
                }
            }
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes() => Enumerable.Empty<PermissionStereotype>();

        /// <summary>
        /// Creates a dynamic permission template for a content type, based on a global content permission template.
        /// </summary>
        public static Permission ConvertToDynamicPermissionTemplate(Permission permission) =>
            GroupPermissionTemplates.ContainsKey(permission.Name) ? GroupPermissionTemplates[permission.Name] : null;

        /// <summary>
        /// Generates a permission dynamically for a content type and editor group.
        /// </summary>
        public static Permission CreateDynamicPermission(
            Permission template, ContentTypeDefinition typeDefinition, string group) =>
            new Permission
            {
                Name = string.Format(template.Name, typeDefinition.Name, group),
                Description = string.Format(template.Description, typeDefinition.DisplayName, group),
                Category = typeDefinition.DisplayName + " Editor Groups",
                ImpliedBy = (template.ImpliedBy ?? new Permission[0]).Select(t => CreateDynamicPermission(t, typeDefinition, group))
            };
    }
}