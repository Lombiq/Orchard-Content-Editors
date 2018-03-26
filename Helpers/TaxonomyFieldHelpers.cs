﻿using Orchard.Taxonomies.Fields;
using Orchard.Taxonomies.Helpers;
using Orchard.Taxonomies.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Lombiq.ContentEditors.Helpers
{
    public static class TaxonomyFieldHelpers
    {
        public static IList<SelectListItem> GetSelectListFromTerms(TaxonomyFieldViewModel viewModel)
        {
            var selectedTerm = viewModel.SelectedTerms.FirstOrDefault();
            var termEntries = viewModel.Terms;
            var leavesOnly = viewModel.Settings.LeavesOnly;
            var selectListItems = new List<SelectListItem>();

            foreach (var entry in termEntries)
            {
                if (!entry.Selectable ||
                    (leavesOnly && termEntries.Any(term => term.Path.Contains(entry.Path + entry.Id))))
                    continue;

                var prefix = leavesOnly ? "" : new String('-', entry.GetLevels() * 2) + " ";

                selectListItems.Add(
                    new SelectListItem
                    {
                        Text = prefix + entry.Name,
                        Value = entry.Id.ToString(),
                        Selected = selectedTerm != null && entry.Name == selectedTerm.Name
                    });
            }

            return selectListItems;
        }
    }
}