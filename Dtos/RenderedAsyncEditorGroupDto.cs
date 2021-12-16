using System.Collections.Generic;

namespace Lombiq.ContentEditors.Dtos
{
    public class RenderedAsyncEditorGroupDto
    {
        public string ContentId { get; set; }
        public string EditorGroup { get; set; }
        public string EditorHtml { get; set; }
        public IEnumerable<AsyncEditorGroupDto> EditorGroups { get; set; }
        public string ValidationSummaryHtml { get; set; }
        public string ScriptsHtml { get; set; }
        public string Message { get; set; }
    }
}
