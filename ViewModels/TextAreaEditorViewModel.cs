namespace Lombiq.ContentEditors.ViewModels
{
    public enum TextAreaEditorFlavors
    {
        TinyMce,
        Default,
    }

    public class TextAreaEditorViewModel : EditorViewModel
    {
        public TextAreaEditorFlavors Flavor { get; set; } = TextAreaEditorFlavors.Default;
    }
}