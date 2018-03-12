namespace Lombiq.ContentEditors.Models
{
    public class AsyncEditorWrapperResult
    {
        public int Id { get; set; }
        public dynamic ContentToDisplay { get; set; }
        public string SuccessMessage { get; set; }
        public bool SuccessfulSave { get; set; }
    }
}