namespace Lombiq.ContentEditors.Models
{
    public class AsyncEditorResult
    {
        public bool Success { get; set; }
        public string ResultMessage { get; set; }
        public string EditorShape { get; set; }
        public string AsyncEditorLoaderId { get; set; }
    }
}