namespace Lombiq.ContentEditors.Dtos
{
    public class AsyncEditorGroupDto
    {
        public string Name { get; set; }
        public string DisplayText { get; set; }
        public bool IsPublishGroup { get; set; }
        public bool IsAccessible { get; set; }
        public bool IsFilled { get; set; }
    }
}