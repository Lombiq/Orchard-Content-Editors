using Orchard.Localization;

namespace Lombiq.ContentEditors.ViewModels
{
    public class PropertyEditorViewModel
    {
        public string PropertyName { get; set; }
        public string Label { get; set; }
        public string Placeholder { get; set; }
        public string Hint { get; set; }
        public object Value { get; set; }
        public string EditorType { get; set; }
        public string TemplateName { get; set; }
        public string Prefix { get; set; }
        public bool Required { get; set; }
        public bool HasOwnTemplate { get; set; }
    }

    
    public static class PropertyViewModelExtensions
    {
        public static PropertyEditorViewModel SetName(this PropertyEditorViewModel viewModel, string propertyName)
        {
            viewModel.PropertyName = propertyName;

            return viewModel;
        }

        public static PropertyEditorViewModel SetLabel(this PropertyEditorViewModel viewModel, LocalizedString label)
        {
            viewModel.Label = label.Text;

            return viewModel;
        }

        public static PropertyEditorViewModel SetPlaceholder(this PropertyEditorViewModel viewModel, LocalizedString placeholder)
        {
            viewModel.Placeholder = placeholder.Text;

            return viewModel;
        }

        public static PropertyEditorViewModel SetHint(this PropertyEditorViewModel viewModel, LocalizedString hint)
        {
            viewModel.Hint = hint.Text;

            return viewModel;
        }

        public static PropertyEditorViewModel SetTemplateName(this PropertyEditorViewModel viewModel, string templateName)
        {
            viewModel.TemplateName = templateName;

            return viewModel;
        }

        public static PropertyEditorViewModel SetPrefix(this PropertyEditorViewModel viewModel, string prefix)
        {
            viewModel.Prefix = prefix;

            return viewModel;
        }

        public static PropertyEditorViewModel SetRequired(this PropertyEditorViewModel viewModel, bool required = true)
        {
            viewModel.Required = required;

            return viewModel;
        }

        public static PropertyEditorViewModel WithOwnTemplate(this PropertyEditorViewModel viewModel, bool hasOwnHemplate = true)
        {
            viewModel.HasOwnTemplate = hasOwnHemplate;

            return viewModel;
        }
    }
}