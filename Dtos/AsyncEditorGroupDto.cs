using Lombiq.ContentEditors.Models;
using System.Collections.Generic;

namespace Lombiq.ContentEditors.Dtos
{
    public class AsyncEditorGroupDto
    {
        public string ContentId { get; set; }
        public string EditorGroup { get; set; }
        public string EditorHtml { get; set; }
        public IEnumerable<AsyncEditorGroup> EditorGroups { get; set; }
    }
}
