using Lombiq.ContentEditors.Models;
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
        public static IList<SelectListItem> GetSelectListFromTermsUnderParent(TaxonomyFieldViewModel viewModel, int? parentId = null, int depth = int.MaxValue)
        {
            var selectedTermName = viewModel.SelectedTerms.FirstOrDefault()?.Name;
            var parentTermLevel = parentId == null ? 0 : viewModel.Terms.Where(term => term.Id == parentId).FirstOrDefault().GetLevels();
            var selectListItems = new List<SelectListItem>();

            foreach (var entry in viewModel.Terms)
            {
                if (!entry.Selectable ||
                    (parentId != null || !entry.Path.Contains("/" + parentId) || (entry.GetLevels() - parentTermLevel) >= depth))
                    continue;

                selectListItems.Add(CreateSelectListItem(entry, selectedTermName, parentTermLevel));
            }

            return selectListItems;
        }

        public static IList<SelectListItem> GetSelectListFromTermsUnderLevel(TaxonomyFieldViewModel viewModel, int startingLevel = 0, int depth = int.MaxValue)
        {
            var selectedTermName = viewModel.SelectedTerms.FirstOrDefault()?.Name;
            var selectListItems = new List<SelectListItem>();

            foreach (var entry in viewModel.Terms)
            {
                if (!entry.Selectable || entry.GetLevels() < startingLevel || (entry.GetLevels() - startingLevel) >= depth)
                    continue;

                selectListItems.Add(CreateSelectListItem(entry, selectedTermName, startingLevel));
            }

            return selectListItems;
        }

        public static IDictionary<string, IEnumerable<ValueNamePair>> CreateAllValueStructuresFromTerms(TaxonomyFieldViewModel viewModel, int parentLevel = 0, int depth = int.MaxValue)
        {
            var termEntries = viewModel.Terms;
            var topLevelTermEntries = termEntries.Where(term => term.GetLevels() == parentLevel).ToList();
            var valueStructures = new Dictionary<string, IEnumerable<ValueNamePair>>();

            foreach (var entry in topLevelTermEntries)
            {
                var children = termEntries.Where(term => term.Path.Contains(entry.Path + entry.Id) && term.GetLevels() <= depth + parentLevel)
                    .Select(term => new ValueNamePair { Value = term.Id.ToString(), Name = term.Name }).ToList();
                valueStructures[entry.Id.ToString()] = children;
            }

            return valueStructures;
        }


        private static SelectListItem CreateSelectListItem(TermEntry term, string selectedTermName, int startingLevel)
        {
            var prefix = new String(' ', (term.GetLevels() - startingLevel) * 2);

            return new SelectListItem
            {
                Text = prefix + term.Name,
                Value = term.Id.ToString(),
                Selected = term.Name == selectedTermName
            };
        }
    }
}