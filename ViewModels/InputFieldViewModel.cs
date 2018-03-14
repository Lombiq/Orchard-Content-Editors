using Orchard.ContentManagement;

namespace Lombiq.ContentEditors.ViewModels
{
    public class InputFieldViewModel : FieldViewModel
    {
        /// <summary>
        /// Value of the HTML field.
        /// </summary>
        public object Value { get; set; }

        /// <summary>
        /// Corresponding content item to the input field.
        /// </summary>
        public ContentItem ContentItem { get; set; }
    }
}