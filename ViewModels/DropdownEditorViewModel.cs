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
        public bool HasDefaultEmptyValue { get; set; }
        public string EmptyValueText { get; set; }
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

        public DropdownEditorViewModel() { }
    }
}