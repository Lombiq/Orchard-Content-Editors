namespace Lombiq.ContentEditors.Models;

public class RenderAsyncEditorRequest
{
    public string AsyncEditorId { get; set; }
    public string ProviderName { get; set; }
    public string ContentType { get; set; }
    public string ContentId { get; set; }
    public string EditorGroup { get; set; }
}
