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
        public RenderMode RenderMode { get; set; } = RenderMode.RadioButtons;
    }
}