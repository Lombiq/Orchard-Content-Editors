using System.Collections.Generic;
using System.Linq;

namespace Lombiq.ContentEditors.ViewModels
{
    public class FieldViewModel
    {
        /// <summary>
        /// Label of the HTML field.
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// Technical name of the UI component.
        /// </summary>
        public string TechnicalName { get; set; }

        /// <summary>
        /// BEM-style name of the UI component.
        /// </summary>
        public string BlockName { get; set; }

        /// <summary>
        /// Additional HTML classes to be added to the HTML editor field.
        /// </summary>
        public string AdditionalClasses { get; set; }

        /// <summary>
        /// Additional HTML attributes to be added to the HTML editor field.
        /// </summary>
        public IDictionary<string, object> AdditionalAttributes { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// If this is set, the field will be hidden if it has no value.
        /// </summary>
        public bool ShowIfEmpty { get; set; }
    }


    public static class FieldViewModelExtensions
    {
        public static IDictionary<string, object> BuildAttributes(
            this FieldViewModel viewModel,
            IDictionary<string, object> customAttributes)
        {
            if (!string.IsNullOrEmpty(viewModel.AdditionalClasses))
            {
                if (customAttributes.ContainsKey("class"))
                {
                    customAttributes["class"] = customAttributes["class"] + " " + viewModel.AdditionalClasses;
                }
                else
                {
                    customAttributes.Add("class", viewModel.AdditionalClasses);
                }
            }

            if (viewModel.AdditionalAttributes != null && viewModel.AdditionalAttributes.Any())
            {
                foreach (var attribute in viewModel.AdditionalAttributes)
                {
                    if (!customAttributes.ContainsKey(attribute.Key))
                    {
                        customAttributes.Add(attribute.Key, attribute.Value);
                    }
                }
            }

            return customAttributes;
        }
    }
}