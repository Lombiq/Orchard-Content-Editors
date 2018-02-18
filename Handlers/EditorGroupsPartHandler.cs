using Lombiq.EditorGroups.Models;
using Orchard.ContentManagement.Handlers;
using Orchard.Services;
using System.Collections.Generic;
using System.Linq;

namespace Lombiq.EditorGroups.Handlers
{
    public class EditorGroupsPartHandler : ContentHandler
    {
        public EditorGroupsPartHandler(IJsonConverter jsonConverter)
        {
            OnActivated<EditorGroupsPart>((context, part) =>
            {
                part.EditorGroupsField.Loader(() => 
                {
                    if (string.IsNullOrEmpty(part.EditorGroupsSerialized))
                    {
                        return Enumerable.Empty<EditorGroupDescriptor>();
                    }

                    return jsonConverter.Deserialize<IEnumerable<EditorGroupDescriptor>>(part.EditorGroupsSerialized);
                });

                part.FilledEditorGroupNamesField.Loader(() =>
                {
                    if (string.IsNullOrEmpty(part.EditorGroupsSerialized))
                    {
                        return Enumerable.Empty<string>();
                    }

                    return jsonConverter.Deserialize<IEnumerable<string>>(part.EditorGroupsSerialized);
                });
            });
        }
    }
}