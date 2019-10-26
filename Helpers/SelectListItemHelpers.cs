using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Lombiq.ContentEditors.Helpers
{
    public static class SelectListItemHelpers
    {
        public static List<SelectListItem> CreateListFromValues(IEnumerable<string> values, string selectedValue = "") =>
            values?.Select(item => new SelectListItem
            {
                Text = item,
                Value = item,
                Selected = selectedValue == item
            }).ToList() ?? new List<SelectListItem>();
    }
}