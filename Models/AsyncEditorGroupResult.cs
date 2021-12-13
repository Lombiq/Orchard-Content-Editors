namespace Lombiq.ContentEditors.Models
{
    public class AsyncEditorGroupResult : AsyncEditorResult
    {
        public int ContentItemId { get; set; }
        public string EditorGroup { get; set; }
    }
}