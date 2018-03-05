using Lombiq.ContentEditors.Models;
using Orchard;
using System.Collections.Generic;

namespace Lombiq.ContentEditors.Services
{
    public interface IAsyncEditorService : IDependency
    {
        /// <summary>
        /// Checks if the user is authorized to edit the given content.
        /// </summary>
        /// <param name="part">AsyncEditorPart of the content item.</param>
        /// <param name="group">Editor group to be authorized on.</param>
        /// <returns>True if the current user is authorized.</returns>
        bool IsAuthorizedToEdit(AsyncEditorPart part, string group = "");

        /// <summary>
        /// Checks if the user is authorized to edit the given content.
        /// </summary>
        /// <param name="part">AsyncEditorPart of the content item.</param>
        /// <param name="group">Editor group to be authorized on.</param>
        /// <returns>True if the current user is authorized.</returns>
        bool IsAuthorizedToPublish(AsyncEditorPart part, string group = "");

        /// <summary>
        /// Returns editor groups that are completed.
        /// </summary>
        /// <param name="part">AsyncEditorPart of the content item.</param>
        /// <param name="authorizedOnly">If this is true, then it operates only with groups that the user is authorized to edit.</param>
        /// <returns>Completed editor groups.</returns>
        IEnumerable<EditorGroupDescriptor> GetCompletedEditorGroups(AsyncEditorPart part, bool authorizedOnly = false);

        /// <summary>
        /// Returns editor groups that are incomplete.
        /// </summary>
        /// <param name="part">AsyncEditorPart of the content item.</param>
        /// <param name="authorizedOnly">If this is true, then it operates only with groups that the user is authorized to edit.</param>
        /// <returns>Incomplete editor groups.</returns>
        IEnumerable<EditorGroupDescriptor> GetIncompleteEditorGroups(AsyncEditorPart part, bool authorizedOnly = false);

        /// <summary>
        /// Returns editor groups that are available to edit (i.e. complete groups or the one after the last complete group).
        /// </summary>
        /// <param name="part">AsyncEditorPart of the content item.</param>
        /// <param name="authorizedOnly">If this is true, then it operates only with groups that the user is authorized to edit.</param>
        /// <returns>Available editor groups.</returns>
        IEnumerable<EditorGroupDescriptor> GetAvailableEditorGroups(AsyncEditorPart part, bool authorizedOnly = false);

        /// <summary>
        /// Returns editor groups that the current user is authorized to edit.
        /// </summary>
        /// <param name="part">AsyncEditorPart of the content item.</param>
        /// <returns>Authorized editor groups.</returns>
        IEnumerable<EditorGroupDescriptor> GetAuthorizedEditorGroups(AsyncEditorPart part);

        /// <summary>
        /// Initializes the given content item to be edited async and also returns the generated editor shape.
        /// </summary>
        /// <param name="part">AsyncEditorPart of the content item.</param>
        /// <param name="group">Optionally the editor group that the editor shape is generated for.</param>
        /// <returns>Editor shape for the given editor group.</returns>
        dynamic BuildAsyncEditorShape(AsyncEditorPart part, string group = "");

        /// <summary>
        /// Returns the editor group details including its technical name and title and other possible options.
        /// </summary>
        /// <param name="part">AsyncEditorPart of the content item.</param>
        /// <param name="group">Technical name of the group that identifies the editor group.</param>
        /// <returns>Editor group details.</returns>
        EditorGroupDescriptor GetEditorGroupDescriptor(AsyncEditorPart part, string group);

        /// <summary>
        /// Returns the next editor group in the sequence after the given group.
        /// </summary>
        /// <param name="part">AsyncEditorPart of the content item.</param>
        /// <param name="group">Name of the group in the sequence.</param>
        /// <param name="authorizedOnly">If this is true, then it operates only with groups that the user is authorized to edit.</param>
        /// <returns>Editor group details after the given group.</returns>
        EditorGroupDescriptor GetNextGroupDescriptor(AsyncEditorPart part, string group, bool authorizedOnly = false);

        /// <summary>
        /// Returns the previous editor group in the sequence before the given group.
        /// </summary>
        /// <param name="part">AsyncEditorPart of the content item.</param>
        /// <param name="group">Name of the group in the sequence.</param>
        /// <param name="authorizedOnly">If this is true, then it operates only with groups that the user is authorized to edit.</param>
        /// <returns>Editor group details before the given group.</returns>
        EditorGroupDescriptor GetPreviousGroupDescriptor(AsyncEditorPart part, string group, bool authorizedOnly = false);

        /// <summary>
        /// Checks if the group is available to edit (i.e. it's a complete group or the one after the last complete group).
        /// </summary>
        /// <param name="part">AsyncEditorPart of the content item.</param>
        /// <param name="group">Name of the group that needs to be checked.</param>
        /// <returns>True if the editor group is available.</returns>
        bool IsEditorGroupAvailable(AsyncEditorPart part, string group);

        /// <summary>
        /// Registers the given editor group as complete.
        /// </summary>
        /// <param name="part">AsyncEditorPart of the content item.</param>
        /// <param name="group">Name of the group that needs to be stored.</param>
        void StoreCompletedEditorGroup(AsyncEditorPart part, string group);

        /// <summary>
        /// Returns the editor group settings including the available editor groups if there is any provider for the given content type.
        /// </summary>
        /// <param name="part">AsyncEditorPart of the content item.</param>
        /// <returns>Editor group settings.</returns>
        EditorGroupsSettings GetEditorGroupsSettings(AsyncEditorPart part);
    }
}