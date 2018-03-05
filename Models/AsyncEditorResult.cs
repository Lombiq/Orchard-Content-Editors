namespace Lombiq.ContentEditors.Models
{
    public class AsyncEditorResult
    {
        public int ContentItemId { get; set; }
        public bool Success { get; set; }
        public string ResultMessage { get; set; }
        public string EditorShape { get; set; }
        public string EditorGroup { get; set; }
    }
}