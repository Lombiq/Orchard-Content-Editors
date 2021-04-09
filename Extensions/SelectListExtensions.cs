using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Lombiq.ContentEditors.Extensions
{
    public static class SelectListExtensions
    {
        public static List<SelectListItem> SortByText(this List<SelectListItem> list, bool orderByAscending = true) =>
            orderByAscending
                ? list.OrderBy(value => value.Text).ToList()
                : list.OrderByDescending(value => value.Text).ToList();

        public static void Move(this List<SelectListItem> list, SelectListItem item, int newIndex)
        {
            if (item != null && newIndex > -1)
            {
                newIndex = Math.Min(newIndex, list.Count - 1);
                var oldIndex = list.IndexOf(item);

                if (oldIndex > -1 && oldIndex != newIndex)
                {
                    list.RemoveAt(oldIndex);

                    list.Insert(newIndex, item);
                }
            }
        }

        public static void MoveItemByValue(this List<SelectListItem> list, string itemValue, int newIndex)
        {
            if (!string.IsNullOrEmpty(itemValue))
            {
                list.Move(list.FirstOrDefault(selectListItem => selectListItem.Value == itemValue), newIndex);
            }
        }
    }
}