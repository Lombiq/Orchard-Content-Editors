using Lombiq.ContentEditors.Models;
using Lombiq.ContentEditors.Services;
using Orchard.ContentManagement.Handlers;
using System.Collections.Generic;
using System.Linq;

namespace Lombiq.ContentEditors.Handlers
{
    public class AsyncEditorPartHandler : ContentHandler
    {
        public AsyncEditorPartHandler(
            IEditorGroupsProviderAccessor editorGroupsProviderAccessor,
            IAsyncEditorService asyncEditorService)
        {
            OnActivated<AsyncEditorPart>((context, part) =>
            {
                part.EditorGroupsSettingsField.Loader(() =>
                    editorGroupsProviderAccessor.GetProvider(part.TypeDefinition.Name)?.GetEditorGroupsSettings());

                part.HasEditorGroupsField.Loader(() => part.EditorGroupsSettings != null);

                part.UnauthorizedEditorGroupBehaviorField.Loader(() =>
                    part.EditorGroupsSettings?.UnauthorizedEditorGroupBehavior ??
                    UnauthorizedEditorGroupBehavior.AllowEditingAllAuthorizedGroups);

                part.AuthorizedEditorGroupsField.Loader(() =>
                {
                    var editorGroups = part.EditorGroupsSettings?.EditorGroups;
                    if (editorGroups == null) return Enumerable.Empty<EditorGroupDescriptor>();

                    var authorizedEditorGroups = new List<EditorGroupDescriptor>();
                    foreach (var editorGroup in editorGroups)
                    {
                        if (asyncEditorService.IsAuthorizedToEdit(part, editorGroup.Name))
                        {
                            authorizedEditorGroups.Add(editorGroup);

                            continue;
                        }

                        if (part.UnauthorizedEditorGroupBehavior ==
                            UnauthorizedEditorGroupBehavior.AllowEditingUntilFirstUnauthorizedGroup)
                        {
                            break;
                        }
                    }

                    return authorizedEditorGroups;
                });

                part.CompletedAuthorizedEditorGroupsField.Loader(() =>
                    asyncEditorService.GetCompletedEditorGroups(part, true));

                part.IncompleteAuthorizedEditorGroupsField.Loader(() =>
                    asyncEditorService.GetIncompleteEditorGroups(part, true));

                part.AvailableAuthorizedEditorGroupsField.Loader(() =>
                    asyncEditorService.GetAvailableEditorGroups(part, true));

                part.NextAuthorizedEditorGroupField.Loader(() =>
                    part.CurrentEditorGroup == null ?
                        null : asyncEditorService.GetNextGroupDescriptor(part, part.CurrentEditorGroup.Name, true));

                part.PreviousAuthorizedEditorGroupField.Loader(() =>
                    part.CurrentEditorGroup == null ?
                        null : asyncEditorService.GetPreviousGroupDescriptor(part, part.CurrentEditorGroup.Name, true));

                part.NextEditableAuthorizedGroupField.Loader(() =>
                    asyncEditorService.GetIncompleteEditorGroups(part, true).FirstOrDefault() ??
                        asyncEditorService.GetCompletedEditorGroups(part, true).LastOrDefault());

                part.LastUpdatedEditorGroupField.Loader(() =>
                    !string.IsNullOrEmpty(part.LastUpdatedEditorGroupName) ?
                        part.AuthorizedEditorGroups.FirstOrDefault(group => group.Name == part.LastUpdatedEditorGroupName) : null);

                part.AreAllEditorGroupsCompletedField.Loader(() =>
                    !asyncEditorService.GetIncompleteEditorGroups(part).Any());

                part.LastDisplayedEditorGroupField.Loader(() =>
                    !string.IsNullOrEmpty(part.LastDisplayedEditorGroupName) ?
                        part.AuthorizedEditorGroups.FirstOrDefault(group => group.Name == part.LastDisplayedEditorGroupName) : null);
            });
        }
    }
}