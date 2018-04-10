﻿using Lombiq.ContentEditors.Models;
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
        public static IList<SelectListItem> GetSelectListFromTerms(TaxonomyFieldViewModel viewModel, int? parentId = null, int depth = int.MaxValue)
        {
            var selectedTerm = viewModel.SelectedTerms.FirstOrDefault();
            var parentTermLevel = parentId == null ? 0 : viewModel.Terms.Where(term => term.Id == parentId).FirstOrDefault().GetLevels();
            var termEntries = viewModel.Terms;
            var selectListItems = new List<SelectListItem>();

            foreach (var entry in termEntries)
            {
                if (!entry.Selectable ||
                    (parentId != null || !entry.Path.Contains("/" + parentId) || (entry.GetLevels() - parentTermLevel) > depth))
                    continue;

                var prefix = new String(' ', (entry.GetLevels() - parentTermLevel) * 2);

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

        public static IDictionary<string, IEnumerable<ValueNamePair>> CreateAllValueStructuresFromTerms(TaxonomyFieldViewModel viewModel)
        {
            var termEntries = viewModel.Terms;
            var topLevelTermEntries = termEntries.Where(term => term.GetLevels() == 0).ToList();
            var valueStructures = new Dictionary<string, IEnumerable<ValueNamePair>>();

            foreach (var entry in topLevelTermEntries)
            {
                var children = termEntries.Where(term => term.Path.Contains(entry.Path + entry.Id))
                    .Select(term => new ValueNamePair { Value = term.Id.ToString(), Name = term.Name }).ToList();
                valueStructures[entry.Id.ToString()] = children;
            }

            return valueStructures;
        }
    }
}