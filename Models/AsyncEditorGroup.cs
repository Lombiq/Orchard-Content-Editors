using Microsoft.Extensions.Localization;

namespace Lombiq.ContentEditors.Models
{
    public class AsyncEditorGroup
    {
        public string Name { get; set; }
        public LocalizedString DisplayText { get; set; }
        public bool IsAccessible { get; set; }
    }
}
