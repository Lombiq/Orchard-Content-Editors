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

                part.NextEditableAuthorizedGroupField.Loader(() =>
                    part.GetIncompleteEditorGroups(true).FirstOrDefault() ?? part.GetCompletedEditorGroups(true).LastOrDefault());
            });
        }
    }
}