using System.Collections.Generic;

namespace Lombiq.ContentEditors.Models;

public class RenderedAsyncEditorGroupRequest
{
    public string ContentId { get; set; }
    public string EditorGroup { get; set; }
    public string EditorHtml { get; set; }
    public IEnumerable<AsyncEditorGroupRequest> EditorGroups { get; set; }
    public string ValidationSummaryHtml { get; set; }
    public string ScriptsHtml { get; set; }
    public string Message { get; set; }
}
