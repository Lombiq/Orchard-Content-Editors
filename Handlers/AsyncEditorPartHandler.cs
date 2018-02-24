using Lombiq.EditorGroups.Models;
using Lombiq.EditorGroups.Services;
using Orchard.ContentManagement.Handlers;
using Orchard.Services;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lombiq.EditorGroups.Handlers
{
    public class AsyncEditorPartHandler : ContentHandler
    {
        public AsyncEditorPartHandler(
            IJsonConverter jsonConverter,
            IAsyncEditorService asyncEditorService)
        {
            OnActivated<AsyncEditorPart>((context, part) =>
            {
                var editorGroupsSettingsLazy = new Lazy<EditorGroupsSettings>(() => 
                    asyncEditorService.GetEditorGroupsSettings(part));

                part.HasEditorGroupsField.Loader(() => editorGroupsSettingsLazy.Value != null);

                part.EditorGroupsField.Loader(() => editorGroupsSettingsLazy.Value.EditorGroups);

                part.UnauthorizedEditorGroupBehaviorField.Loader(() => editorGroupsSettingsLazy.Value.UnauthorizedEditorGroupBehavior);

                part.AuthorizedEditorGroupsField.Loader(() => asyncEditorService.GetAuthorizedEditorGroups(part));

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

                part.NextEditorGroupField.Loader(() =>
                    part.CurrentEditorGroup == null ?
                        null :
                        asyncEditorService.GetNextAuthorizedGroupDescriptor(part, part.CurrentEditorGroup.Name));

                part.PreviousEditorGroupField.Loader(() =>
                    part.CurrentEditorGroup == null ?
                        null :
                        asyncEditorService.GetPreviousAuthorizedGroupDescriptor(part, part.CurrentEditorGroup.Name));
            });
        }
    }
}