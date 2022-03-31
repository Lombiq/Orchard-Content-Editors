namespace Lombiq.ContentEditors.Models;

public class AsyncEditorContext<T>
{
    public T Content { get; set; }
    public string EditorGroup { get; set; }
    public string AsyncEditorId { get; set; }
}
