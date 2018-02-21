using Lombiq.EditorGroups.Models;
using Lombiq.EditorGroups.Services;
using Orchard.ContentManagement.Handlers;
using Orchard.Services;
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
                part.EditorGroupsField.Loader(() => 
                {
                    var provider = editorGroupProvidersAccessor.GetProvider(part.ContentItem.ContentType);

                    if (provider == null) return Enumerable.Empty<EditorGroupDescriptor>();

                    return provider.GetEditorGroups();
                });

                part.AuthorizedEditorGroupsField.Loader(() =>
                {
                    var authorizedEditorGroups = new List<EditorGroupDescriptor>();
                    foreach (var editorGroup in part.EditorGroups)
                    {
                        if (!asyncEditorService.IsAuthorizedToEditGroup(part, editorGroup.Name)) break;

                        authorizedEditorGroups.Add(editorGroup);
                    }

                    return authorizedEditorGroups;
                });

                part.FilledEditorGroupNamesField.Loader(() =>
                {
                    if (string.IsNullOrEmpty(part.FilledEditorGroupNamesSerialized))
                    {
                        return Enumerable.Empty<string>();
                    }

                    return jsonConverter.Deserialize<IEnumerable<string>>(part.FilledEditorGroupNamesSerialized);
                });

                part.UnfilledEditorGroupNamesField.Loader(() => 
                    part.EditorGroups.Select(editorGroup => editorGroup.Name).Except(part.FilledEditorGroupNames));
            });
        }
    }
}