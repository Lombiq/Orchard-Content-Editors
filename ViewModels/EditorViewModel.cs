﻿namespace Lombiq.ContentEditors.ViewModels
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
        /// Hint text to be displayed next to the HTML field.
        /// </summary>
        public string Hint { get; set; }

        /// <summary>
        /// Indicates whether the HTML field is required. It's only used to set specific classes
        /// that triggers front-end validation logic.
        /// </summary>
        public bool Required { get; set; }
    }
}