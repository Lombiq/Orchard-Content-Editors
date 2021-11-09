namespace Lombiq.ContentEditors.ViewModels
{
    public class EditorViewModel : InputFieldViewModel
    {
        /// <summary>
        /// Name of the HTML field.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Placeholder text to be shown on the HTML field.
        /// </summary>
        public string Placeholder { get; set; }

        /// <summary>
        /// Description text to be displayed above the HTML field.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Additional text to be displayed when hovering over an element after the field name.
        /// </summary>
        public string Tooltip { get; set; }

        /// <summary>
        /// Hint text to be displayed under the HTML field.
        /// </summary>
        public string Hint { get; set; }

        /// <summary>
        /// Indicates whether the HTML field is required. It's only used to set specific classes
        /// that triggers front-end validation logic.
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Indicates whether the HTML field is Disabled.
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// Indicates whether html is allowed in tooltip text.
        /// </summary>
        public bool AllowTooltipHtml { get; set; }

        /// <summary>
        /// Text to be displayed under the HTML field.
        /// </summary>
        public string TextUnderField { get; set; }
    }
}