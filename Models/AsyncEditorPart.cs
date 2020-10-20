﻿using Orchard.ContentManagement;
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
            get => CompletedEditorGroupNamesField.Value ?? Enumerable.Empty<string>();
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

        internal LazyField<UnauthorizedEditorGroupBehavior> UnauthorizedEditorGroupBehaviorField { get; } = new LazyField<UnauthorizedEditorGroupBehavior>();
        public UnauthorizedEditorGroupBehavior UnauthorizedEditorGroupBehavior => UnauthorizedEditorGroupBehaviorField.Value;

        internal LazyField<IEnumerable<EditorGroupDescriptor>> AuthorizedEditorGroupsField { get; } = new LazyField<IEnumerable<EditorGroupDescriptor>>();
        /// <summary>
        /// Returns editor groups that the current user is authorized to edit.
        /// </summary>
        /// <param name="part">AsyncEditorPart of the content item.</param>
        /// <returns>Authorized editor groups.</returns>
        public IEnumerable<EditorGroupDescriptor> AuthorizedEditorGroups => AuthorizedEditorGroupsField.Value;

        internal LazyField<EditorGroupDescriptor> NextEditableAuthorizedGroupField { get; } = new LazyField<EditorGroupDescriptor>();
        public EditorGroupDescriptor NextEditableAuthorizedGroup => NextEditableAuthorizedGroupField.Value;
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
        public static EditorGroupDescriptor GetEditorGroupDescriptor(this AsyncEditorPart part, string group) =>
            part.EditorGroupsSettings?.EditorGroups.FirstOrDefault(editorGroup => editorGroup.Name == group);

        public static void SetCurrentEditorGroup(this AsyncEditorPart part, string group) =>
            part.CurrentEditorGroup = part.GetEditorGroupDescriptor(group);

        /// <summary>
        /// Registers the given editor group as complete.
        /// </summary>
        /// <param name="part">AsyncEditorPart of the content item.</param>
        /// <param name="group">Name of the group that needs to be stored.</param>
        public static void StoreCompletedEditorGroup(this AsyncEditorPart part, string group)
        {
            Argument.ThrowIfNullOrEmpty(group, nameof(group));

            if (!part.EditorGroupsSettings?.EditorGroups.Any(editorGroup => editorGroup.Name == group) ?? false) return;

            part.CompletedEditorGroupNames = part.GetCompletedEditorGroups()
                .Select(editorGroup => editorGroup.Name)
                .Union(new[] { group })
                .Distinct();
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

            return editorGroups == null ?
                Enumerable.Empty<EditorGroupDescriptor>() : editorGroups.Except(part.GetCompletedEditorGroups(authorizedOnly));
        }

        /// <summary>
        /// Checks if the group is available to edit (i.e. it's a complete group or the one after the last complete group).
        /// </summary>
        /// <param name="part">AsyncEditorPart of the content item.</param>
        /// <param name="group">Name of the group that needs to be checked.</param>
        /// <returns>True if the editor group is available.</returns>
        public static bool IsEditorGroupAvailable(this AsyncEditorPart part, string group)
        {
            Argument.ThrowIfNullOrEmpty(group, nameof(group));

            var editorGroup = part.GetEditorGroupDescriptor(group);

            return editorGroup != null &&
                (part.GetCompletedEditorGroups().Contains(editorGroup) || editorGroup.Equals(part.GetIncompleteEditorGroups().FirstOrDefault()));
        }

        /// <summary>
        /// Returns editor groups that are available to edit (i.e. complete groups or the one after the last complete group).
        /// </summary>
        /// <param name="part">AsyncEditorPart of the content item.</param>
        /// <param name="authorizedOnly">If this is true, then it operates only with groups that the user is authorized to edit.</param>
        /// <returns>Available editor groups.</returns>
        public static IEnumerable<EditorGroupDescriptor> GetAvailableEditorGroups(this AsyncEditorPart part, bool authorizedOnly = false)
        {
            var editorGroups = part.GetEditorGroupList(authorizedOnly);
            if (editorGroups == null) return Enumerable.Empty<EditorGroupDescriptor>();

            return editorGroups.Where(editorGroup => part.IsEditorGroupAvailable(editorGroup.Name));
        }

        /// <summary>
        /// Returns the next editor group in the sequence after the given group.
        /// </summary>
        /// <param name="part">AsyncEditorPart of the content item.</param>
        /// <param name="group">Name of the group in the sequence.</param>
        /// <param name="authorizedOnly">If this is true, then it operates only with groups that the user is authorized to edit.</param>
        /// <returns>Editor group details after the given group.</returns>
        public static EditorGroupDescriptor GetNextGroupDescriptor(this AsyncEditorPart part, string group, bool authorizedOnly = false)
        {
            if (string.IsNullOrEmpty(group)) return null;

            var editorGroups = part.GetEditorGroupList(authorizedOnly);

            if (!editorGroups?.Any() ?? false) return null;

            var groupDescriptor = editorGroups.FirstOrDefault(editorGroup => editorGroup.Name == group);

            return groupDescriptor == null ?
                null :
                groupDescriptor == editorGroups.Last() ?
                    null :
                    editorGroups[editorGroups.IndexOf(groupDescriptor) + 1];
        }

        /// <summary>
        /// Returns the previous editor group in the sequence before the given group.
        /// </summary>
        /// <param name="part">AsyncEditorPart of the content item.</param>
        /// <param name="group">Name of the group in the sequence.</param>
        /// <param name="authorizedOnly">If this is true, then it operates only with groups that the user is authorized to edit.</param>
        /// <returns>Editor group details before the given group.</returns>
        public static EditorGroupDescriptor GetPreviousGroupDescriptor(this AsyncEditorPart part, string group, bool authorizedOnly = false)
        {
            if (string.IsNullOrEmpty(group)) return null;

            var editorGroups = part.GetEditorGroupList(authorizedOnly);

            if (!editorGroups?.Any() ?? false) return null;

            var groupDescriptor = editorGroups.FirstOrDefault(editorGroup => editorGroup.Name == group);

            return groupDescriptor == null ?
                null :
                groupDescriptor == editorGroups.First() ?
                    null :
                    editorGroups[editorGroups.IndexOf(groupDescriptor) - 1];
        }

        public static EditorGroupDescriptor GetLastUpdatedGroupDescriptor(this AsyncEditorPart part) =>
            part.AuthorizedEditorGroups.FirstOrDefault(group => group.Name == part.LastUpdatedEditorGroupName);

        public static EditorGroupDescriptor GetLastDisplayedGroupDescriptor(this AsyncEditorPart part) =>
            part.AuthorizedEditorGroups.FirstOrDefault(group => group.Name == part.LastDisplayedEditorGroupName);

        public static string GetFallbackEditorGroupName(this AsyncEditorPart part) =>
            part.GetLastDisplayedGroupDescriptor()?.Name ?? part.NextEditableAuthorizedGroup?.Name;
    }
}