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

        public static List<SelectListItem> CreateListFromDictionary(IDictionary<int, string> dictionary, int selectedKey) =>
            dictionary?.Select(item => new SelectListItem
            {
                Text = item.Value,
                Value = item.Key.ToString(),
                Selected = selectedKey == item.Key
            }).ToList() ?? new List<SelectListItem>();
    }
}