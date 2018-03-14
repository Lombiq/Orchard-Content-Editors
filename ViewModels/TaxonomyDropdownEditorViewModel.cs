using System.Collections.Generic;
using System.Web.Mvc;

namespace Lombiq.ContentEditors.ViewModels
{
    public class TaxonomyDropdownEditorViewModel : EditorViewModel
    {
        public string TechnicalName { get; set; }
        public List<SelectListItem> SelectList { get; set; } = new List<SelectListItem>();


        public TaxonomyDropdownEditorViewModel(IEnumerable<string> values)
        {
            foreach (var value in values) SelectList.Add(new SelectListItem { Text = value, Value = value });
        }
    }
}