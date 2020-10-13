using Orchard.Localization;
using Piedone.HelpfulExtensions;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;

namespace Lombiq.ContentEditors.ViewModels
{
    public class DropdownEditorViewModel : EditorViewModel, IParentElementValueDependency, ISelectSpecialValues
    {
        public List<SelectListItem> SelectList { get; set; } = new List<SelectListItem>();
        public bool SingleChoice { get; set; }
        public bool HasDefaultEmptyValue { get; set; }
        public string EmptyValueText { get; set; } = "";
        public string DefaultEmptyValue { get; set; } = "";
        public Dictionary<string, string> Hints { get; set; }
        public Dictionary<string, string> Descriptions { get; set; }

        #region IParentElementValueDependency implementation

        public string ParentElementSelector { get; set; }
        public Dictionary<string, IEnumerable<string>> ParentElementValueHierarchy { get; set; } = new Dictionary<string, IEnumerable<string>>();

        #endregion

        #region ISelectSpecialValues implementation

        public string NoneValueId { get; set; } = "";
        public string OtherValueId { get; set; } = "";

        #endregion


        public DropdownEditorViewModel() { }

        public DropdownEditorViewModel(IEnumerable<string> values, params string[] selectedValues)
        {
            foreach (var value in values) SelectList.Add(new SelectListItem
            {
                Text = value,
                Value = value,
                Selected = selectedValues?.Contains(value) ?? false
            });
        }

        public DropdownEditorViewModel(NameValueCollection queryString, string technicalName, IEnumerable<string> values)
        {
            var selectedValues = queryString.GetQueryStringParameterValues(technicalName) ?? Enumerable.Empty<string>();

            foreach (var value in values)
                SelectList.Add(new SelectListItem
                {
                    Text = value,
                    Value = value,
                    Selected = selectedValues.Contains(value)
                });

            Name = TechnicalName = technicalName;
        }
    }


    public static class DropdownEditorViewModelExtensions
    {
        public static DropdownEditorViewModel WithDefaultBooleanOptions(
            this DropdownEditorViewModel viewModel,
            LocalizedString trueLabel,
            LocalizedString falseLabel,
            bool? selectedValue)
        {
            viewModel.SelectList = new[] {
                new SelectListItem() { Text = trueLabel.Text, Value = "True", Selected = selectedValue == true },
                new SelectListItem() { Text = falseLabel.Text, Value = "False", Selected = selectedValue == false }
            }.ToList();

            return viewModel;
        }
    }
}