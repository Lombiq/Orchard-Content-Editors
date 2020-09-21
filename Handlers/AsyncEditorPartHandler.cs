using Lombiq.ContentEditors.Models;
using Lombiq.ContentEditors.Services;
using Orchard.ContentManagement.Handlers;
using Orchard.ContentManagement.Utilities;
using Orchard.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lombiq.ContentEditors.Handlers
{
    public class AsyncEditorPartHandler : ContentHandler
    {
        public AsyncEditorPartHandler(
            Lazy<IEditorGroupsProviderAccessor> editorGroupsProviderAccessorLazy,
            Lazy<IAsyncEditorService> asyncEditorServiceLazy,
            Lazy<IJsonConverter> jsonConverterLazy)
        {
            OnActivated<AsyncEditorPart>((context, part) =>
            {
                part.CompletedEditorGroupNamesField.SetJsonGetterAndSetter(
                    jsonConverterLazy.Value,
                    () => part.CompletedEditorGroupNamesSerialized,
                    serialized => part.CompletedEditorGroupNamesSerialized = serialized);

                part.EditorGroupsSettingsField.Loader(() =>
                    editorGroupsProviderAccessorLazy.Value.GetProvider(part.TypeDefinition.Name)?.GetEditorGroupsSettings());

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
                        if (asyncEditorServiceLazy.Value.IsAuthorizedToEdit(part, editorGroup.Name))
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
                    asyncEditorServiceLazy.Value.GetCompletedEditorGroups(part, true));

                part.IncompleteAuthorizedEditorGroupsField.Loader(() =>
                    asyncEditorServiceLazy.Value.GetIncompleteEditorGroups(part, true));

                part.AvailableAuthorizedEditorGroupsField.Loader(() =>
                    asyncEditorServiceLazy.Value.GetAvailableEditorGroups(part, true));

                part.NextAuthorizedEditorGroupField.Loader(() =>
                    part.CurrentEditorGroup == null ?
                        null : asyncEditorServiceLazy.Value.GetNextGroupDescriptor(part, part.CurrentEditorGroup.Name, true));

                part.PreviousAuthorizedEditorGroupField.Loader(() =>
                    part.CurrentEditorGroup == null ?
                        null : asyncEditorServiceLazy.Value.GetPreviousGroupDescriptor(part, part.CurrentEditorGroup.Name, true));

                part.NextEditableAuthorizedGroupField.Loader(() =>
                    asyncEditorServiceLazy.Value.GetIncompleteEditorGroups(part, true).FirstOrDefault() ??
                        asyncEditorServiceLazy.Value.GetCompletedEditorGroups(part, true).LastOrDefault());

                part.LastUpdatedEditorGroupField.Loader(() =>
                    !string.IsNullOrEmpty(part.LastUpdatedEditorGroupName) ?
                        part.AuthorizedEditorGroups.FirstOrDefault(group => group.Name == part.LastUpdatedEditorGroupName) : null);

                part.AreAllEditorGroupsCompletedField.Loader(() =>
                    !asyncEditorServiceLazy.Value.GetIncompleteEditorGroups(part).Any());

                part.LastDisplayedEditorGroupField.Loader(() =>
                    !string.IsNullOrEmpty(part.LastDisplayedEditorGroupName) ?
                        part.AuthorizedEditorGroups.FirstOrDefault(group => group.Name == part.LastDisplayedEditorGroupName) : null);
            });
        }
    }
}