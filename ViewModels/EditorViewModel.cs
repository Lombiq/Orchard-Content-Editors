using System.Collections.Generic;
using System.Linq;

namespace Lombiq.ContentEditors.ViewModels
{
    public class EditorViewModel
    {
        public string Label { get; set; }
        public string Placeholder { get; set; }
        public string Name { get; set; }
        public object Value { get; set; }
        public bool Required { get; set; }
        public string Hint { get; set; }
        public string BlockName { get; set; }
        public string AdditionalClasses { get; set; }
        public IDictionary<string, object> AdditionalAttributes { get; set; }


        public EditorViewModel() => 
            AdditionalAttributes = new Dictionary<string, object>();
    }


    public static class EditorViewModelExtensions
    {
        public static IDictionary<string, object> BuildAttributes(
            this EditorViewModel viewModel, 
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