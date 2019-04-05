using Piedone.HelpfulExtensions;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web.Mvc;

namespace Lombiq.ContentEditors.ViewModels
{
    public class DropdownEditorViewModel : EditorViewModel, IParentElementValueDependency
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


        public DropdownEditorViewModel(IEnumerable<string> values)
        {
            foreach (var value in values) SelectList.Add(new SelectListItem { Text = value, Value = value });
        }

        public DropdownEditorViewModel(NameValueCollection queryString, string technicalName, IEnumerable<string> values)
        {
            var selectedValue = queryString.GetQueryStringParameterValues(technicalName)?.FirstOrDefault() ?? "";

            foreach (var value in values)
                SelectList.Add(new SelectListItem { Text = value, Value = value, Selected = value == selectedValue });

            Name = TechnicalName = technicalName;
        }

        public DropdownEditorViewModel() { }
    }
}