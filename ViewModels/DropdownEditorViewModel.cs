using System.Collections.Generic;
using System.Web.Mvc;

namespace Lombiq.ContentEditors.ViewModels
{
    public class DropdownEditorViewModel : EditorViewModel
    {
        public List<SelectListItem> SelectList { get; set; } = new List<SelectListItem>();
        public bool HasDefaultEmptyValue { get; set; }
        public string ParentDropdownEditorName { get; set; }
        public Dictionary<string, Dictionary<string, string>> ValueHierarchy { get; set; }
        public Dictionary<string, string> Hints { get; set; }
        public Dictionary<string, string> Descriptions { get; set; }

        public DropdownEditorViewModel(IEnumerable<string> values)
        {
            foreach (var value in values) SelectList.Add(new SelectListItem { Text = value, Value = value });
        }

        public DropdownEditorViewModel()
        {
        }
    }
}