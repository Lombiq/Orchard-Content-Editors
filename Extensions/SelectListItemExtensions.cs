using Lombiq.ContentEditors.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Lombiq.ContentEditors.Extensions
{
    public static class SelectListItemExtensions
    {
        public static List<SelectListItem> OrderListValues(this List<SelectListItem> selectList, OrderBy orderBy)
        {
            switch (orderBy)
            {
                case OrderBy.Ascending:
                    selectList = selectList.OrderBy(value => value.Text).ToList();
                    break;
                case OrderBy.Descending:
                    selectList = selectList.OrderByDescending(value => value.Text).ToList();
                    break;
                case OrderBy.None:
                    break;
                default:
                    break;
            }

            return selectList;
        }

        public static void Move(this List<SelectListItem> list, SelectListItem item, int newIndex)
        {
            if (item != null)
            {
                var oldIndex = list.IndexOf(item);
                if (oldIndex > -1)
                {
                    list.RemoveAt(oldIndex);

                    // The actual index could have shifted due to the removal.
                    if (newIndex > oldIndex) newIndex--;

                    list.Insert(newIndex, item);
                }
            }

        }

        public static void MoveIfItemByTextIsAvailable(this List<SelectListItem> list, string itemToMoveText, int newIndex)
        {
            if (!string.IsNullOrEmpty(itemToMoveText) &&
                list.Select(selectListItem => selectListItem.Text).Contains(itemToMoveText))
            {
                list.Move(list.FirstOrDefault(selectListItem => selectListItem.Text == itemToMoveText), newIndex);
            }
        }
    }
}