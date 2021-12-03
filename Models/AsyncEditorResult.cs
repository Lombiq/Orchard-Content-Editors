namespace Lombiq.ContentEditors.Models
{
    public class AsyncEditorResult
    {
        public bool Success { get; set; }
        public string ResultMessage { get; set; }
        public string EditorShape { get; set; }
        public int ContentItemId { get; set; }
        public string ContentType { get; set; }
        public string EditUrl { get; set; }
        public bool UseStaticIndicator { get; set; }
    }
}