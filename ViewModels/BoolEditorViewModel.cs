namespace Lombiq.ContentEditors.ViewModels
{
    public enum RenderMode
    {
        RadioButtons,
        Toggle,
        Checkbox
    }


    public class BoolEditorViewModel : EditorViewModel
    {
        public string Id { get; set; }
        public RenderMode RenderMode { get; set; } = RenderMode.RadioButtons;
    }
}