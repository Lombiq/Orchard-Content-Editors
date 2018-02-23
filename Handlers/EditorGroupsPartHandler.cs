using Lombiq.EditorGroups.Models;
using Lombiq.EditorGroups.Services;
using Orchard.ContentManagement.Handlers;
using Orchard.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lombiq.EditorGroups.Handlers
{
    public class EditorGroupsPartHandler : ContentHandler
    {
        public EditorGroupsPartHandler(
            IJsonConverter jsonConverter, 
            IEditorGroupsProviderAccessor editorGroupProvidersAccessor,
            IAsyncEditorService asyncEditorService)
        {
            OnActivated<EditorGroupsPart>((context, part) =>
            {
                var editorGroupsSettingsLazy = new Lazy<EditorGroupsSettings>(() => 
                    editorGroupProvidersAccessor.GetProvider(part.ContentItem.ContentType).GetEditorGroupsSettings());

                part.EditorGroupsField.Loader(() => editorGroupsSettingsLazy.Value.EditorGroups);

                part.UnauthorizedEditorGroupBehaviorField.Loader(() => editorGroupsSettingsLazy.Value.UnauthorizedEditorGroupBehavior);

                part.AuthorizedEditorGroupsField.Loader(() => asyncEditorService.GetAuthorizedGroups(part));

                part.CompleteEditorGroupNamesField.Loader(() =>
                {
                    if (string.IsNullOrEmpty(part.CompleteEditorGroupNamesSerialized))
                    {
                        return Enumerable.Empty<string>();
                    }

                    return jsonConverter.Deserialize<IEnumerable<string>>(part.CompleteEditorGroupNamesSerialized);
                });

                part.CompleteEditorGroupNamesField.Setter(value =>
                {
                    part.CompleteEditorGroupNamesSerialized = jsonConverter.Serialize(value);

                    return value;
                });

                part.IncompleteEditorGroupNamesField.Loader(() => 
                    part.EditorGroups.Select(editorGroup => editorGroup.Name).Except(part.CompleteEditorGroupNames));
            });
        }
    }
}