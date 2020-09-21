using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;
using Orchard.Validation;
using Piedone.HelpfulLibraries.Contents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace Lombiq.ContentEditors.Models
{
    public class AsyncEditorPart : ContentPart
    {
        public bool IsAsyncEditorContext { get; set; }
        public EditorGroupDescriptor CurrentEditorGroup { get; set; }
        public bool IsContentCreationFailed { get; set; }

        #region List of the names of the completed Editor Groups stored as JSON.

        public string CompletedEditorGroupNamesSerialized
        {
            get { return this.Retrieve(x => x.CompletedEditorGroupNamesSerialized); }
            set { this.Store(x => x.CompletedEditorGroupNamesSerialized, value); }
        }

        internal LazyField<IEnumerable<string>> CompletedEditorGroupNamesField { get; } = new LazyField<IEnumerable<string>>();
        public IEnumerable<string> CompletedEditorGroupNames
        {
            get => CompletedEditorGroupNamesField.Value;
            set => CompletedEditorGroupNamesField.Value = value;
        }

        #endregion

        public string LastUpdatedEditorGroupName
        {
            get { return this.Retrieve(x => x.LastUpdatedEditorGroupName); }
            set { this.Store(x => x.LastUpdatedEditorGroupName, value); }
        }

        public string LastDisplayedEditorGroupName
        {
            get { return this.Retrieve(x => x.LastDisplayedEditorGroupName); }
            set { this.Store(x => x.LastDisplayedEditorGroupName, value); }
        }

        public string EditorSessionSalt
        {
            get => this.Retrieve(x => x.EditorSessionSalt, () =>
            {
                // Generating a random cryptographically secure salt for generating content item-specific
                // tokens for cookies that will identify the editor session if anonymous users have permission
                // to edit the content item.
                var saltBytes = new byte[0x10];
                using (var cryptoService = new RNGCryptoServiceProvider()) cryptoService.GetBytes(saltBytes);

                var salt = Convert.ToBase64String(saltBytes);
                EditorSessionSalt = salt;

                return salt;
            });
            set => this.Store(x => x.EditorSessionSalt, value);
        }


        internal LazyField<EditorGroupsSettings> EditorGroupsSettingsField { get; } = new LazyField<EditorGroupsSettings>();
        /// <summary>
        /// Returns the editor group settings including the available editor groups if there is any provider for the given content type.
        /// </summary>
        /// <param name="part">AsyncEditorPart of the content item.</param>
        /// <returns>Editor group settings.</returns>
        public EditorGroupsSettings EditorGroupsSettings => EditorGroupsSettingsField.Value;

        internal LazyField<bool> HasEditorGroupsField { get; } = new LazyField<bool>();
        public bool HasEditorGroups => HasEditorGroupsField.Value;

        internal LazyField<bool> AreAllEditorGroupsCompletedField { get; } = new LazyField<bool>();
        public bool AreAllEditorGroupsCompleted => AreAllEditorGroupsCompletedField.Value;

        internal LazyField<UnauthorizedEditorGroupBehavior> UnauthorizedEditorGroupBehaviorField { get; } = new LazyField<UnauthorizedEditorGroupBehavior>();
        public UnauthorizedEditorGroupBehavior UnauthorizedEditorGroupBehavior => UnauthorizedEditorGroupBehaviorField.Value;

        #region Lazy fields for retrieving multiple Editor Group Desriptors based on certain conditions.

        internal LazyField<IEnumerable<EditorGroupDescriptor>> CompletedAuthorizedEditorGroupsField { get; } = new LazyField<IEnumerable<EditorGroupDescriptor>>();
        public IEnumerable<EditorGroupDescriptor> CompletedAuthorizedEditorGroups => CompletedAuthorizedEditorGroupsField.Value;

        internal LazyField<IEnumerable<EditorGroupDescriptor>> IncompleteAuthorizedEditorGroupsField { get; } = new LazyField<IEnumerable<EditorGroupDescriptor>>();
        public IEnumerable<EditorGroupDescriptor> IncompleteAuthorizedEditorGroups => IncompleteAuthorizedEditorGroupsField.Value;

        internal LazyField<IEnumerable<EditorGroupDescriptor>> AuthorizedEditorGroupsField { get; } = new LazyField<IEnumerable<EditorGroupDescriptor>>();
        /// <summary>
        /// Returns editor groups that the current user is authorized to edit.
        /// </summary>
        /// <param name="part">AsyncEditorPart of the content item.</param>
        /// <returns>Authorized editor groups.</returns>
        public IEnumerable<EditorGroupDescriptor> AuthorizedEditorGroups => AuthorizedEditorGroupsField.Value;

        internal LazyField<IEnumerable<EditorGroupDescriptor>> AvailableAuthorizedEditorGroupsField { get; } = new LazyField<IEnumerable<EditorGroupDescriptor>>();
        public IEnumerable<EditorGroupDescriptor> AvailableAuthorizedEditorGroups => AvailableAuthorizedEditorGroupsField.Value;

        #endregion

        #region Lazy fields for retrieving an Editor Group Descriptor for a specific scenario.

        internal LazyField<EditorGroupDescriptor> NextAuthorizedEditorGroupField { get; } = new LazyField<EditorGroupDescriptor>();
        public EditorGroupDescriptor NextAuthorizedEditorGroup => NextAuthorizedEditorGroupField.Value;

        internal LazyField<EditorGroupDescriptor> PreviousAuthorizedEditorGroupField { get; } = new LazyField<EditorGroupDescriptor>();
        public EditorGroupDescriptor PreviousAuthorizedEditorGroup => PreviousAuthorizedEditorGroupField.Value;

        internal LazyField<EditorGroupDescriptor> NextEditableAuthorizedGroupField { get; } = new LazyField<EditorGroupDescriptor>();
        public EditorGroupDescriptor NextEditableAuthorizedGroup => NextEditableAuthorizedGroupField.Value;

        internal LazyField<EditorGroupDescriptor> LastUpdatedEditorGroupField { get; } = new LazyField<EditorGroupDescriptor>();
        public EditorGroupDescriptor LastUpdatedEditorGroup => LastUpdatedEditorGroupField.Value;

        internal LazyField<EditorGroupDescriptor> LastDisplayedEditorGroupField { get; } = new LazyField<EditorGroupDescriptor>();
        public EditorGroupDescriptor LastDisplayedEditorGroup => LastDisplayedEditorGroupField.Value;

        #endregion
    }


    public static class AsyncEditorPartExtensions
    {
        public static AsyncEditorPart AsAsyncEditorPart(this IContent content) =>
            content?.As<AsyncEditorPart>();

        public static AsyncEditorPart AsAsyncEditorPartOrThrow(this IContent content) =>
            content.AsOrThrow<AsyncEditorPart>();

        public static IList<EditorGroupDescriptor> GetEditorGroupList(this AsyncEditorPart part, bool authorizedOnly = false) =>
            authorizedOnly ?
                part.AuthorizedEditorGroups.ToList() :
                part.EditorGroupsSettings?.EditorGroups.ToList();

        /// <summary>
        /// Returns the editor group details including its technical name and title and other possible options.
        /// </summary>
        /// <param name="part">AsyncEditorPart of the content item.</param>
        /// <param name="group">Technical name of the editor group.</param>
        /// <returns>Editor group details.</returns>
        public static EditorGroupDescriptor GetEditorGroupDescriptor(this AsyncEditorPart part, string group)
        {
            Argument.ThrowIfNullOrEmpty(group, nameof(group));

            return part.EditorGroupsSettings?.EditorGroups.FirstOrDefault(editorGroup => editorGroup.Name == group);
        }

        public static void SetCurrentEditorGroup(this AsyncEditorPart part, string group)
        {
            Argument.ThrowIfNullOrEmpty(group, nameof(group));

            part.CurrentEditorGroup = part.GetEditorGroupDescriptor(group);
        }

        /// <summary>
        /// Returns editor groups that are completed.
        /// </summary>
        /// <param name="part">AsyncEditorPart of the content item.</param>
        /// <param name="authorizedOnly">If this is true, then it operates only with groups that the user is authorized to edit.</param>
        /// <returns>Completed editor groups.</returns>
        public static IEnumerable<EditorGroupDescriptor> GetCompletedEditorGroups(this AsyncEditorPart part, bool authorizedOnly = false)
        {
            var editorGroups = part.GetEditorGroupList(authorizedOnly);
            if (editorGroups == null) return Enumerable.Empty<EditorGroupDescriptor>();

            var completeGroupNames = part.CompletedEditorGroupNames;

            return completeGroupNames
                .Select(groupName => editorGroups.FirstOrDefault(group => group.Name == groupName))
                .Where(group => group != null);
        }

        /// <summary>
        /// Returns editor groups that are incomplete.
        /// </summary>
        /// <param name="part">AsyncEditorPart of the content item.</param>
        /// <param name="authorizedOnly">If this is true, then it operates only with groups that the user is authorized to edit.</param>
        /// <returns>Incomplete editor groups.</returns>
        public static IEnumerable<EditorGroupDescriptor> GetIncompleteEditorGroups(this AsyncEditorPart part, bool authorizedOnly = false)
        {
            var editorGroups = part.GetEditorGroupList(authorizedOnly);
            if (editorGroups == null) return Enumerable.Empty<EditorGroupDescriptor>();

            return editorGroups.Except(part.GetCompletedEditorGroups());
        }
    }
}