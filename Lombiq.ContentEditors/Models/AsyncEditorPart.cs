using OrchardCore.ContentManagement;
using System.Collections.Generic;

namespace Lombiq.ContentEditors.Models
{
    public class AsyncEditorPart : ContentPart
    {
        public IDictionary<string, IEnumerable<string>> FilledEditorGroups { get; } = new Dictionary<string, IEnumerable<string>>();
    }
}
