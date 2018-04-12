using Orchard.Taxonomies.Helpers;
using Orchard.Taxonomies.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Lombiq.ContentEditors.Helpers
{
    public static class TaxonomyFieldHelpers
    {
        public static IList<SelectListItem> GetSelectListFromTermsUnderParent(
            TaxonomyFieldViewModel viewModel, int? parentId = null, int depth = int.MaxValue)
        {
            var selectedTermName = viewModel.SelectedTerms.FirstOrDefault()?.Name;
            var parentTermLevel = parentId == null ? 0 : viewModel.Terms.Where(term => term.Id == parentId).FirstOrDefault().GetLevels();
            var selectListItems = new List<SelectListItem>();

            foreach (var entry in viewModel.Terms)
            {
                if (!entry.Selectable ||
                    (parentId == null || !entry.Path.Contains("/" + parentId) || (entry.GetLevels() - parentTermLevel) > depth))
                    continue;

                selectListItems.Add(CreateSelectListItem(entry, selectedTermName, entry.GetLevels() - parentTermLevel));
            }

            return selectListItems;
        }

        public static IList<SelectListItem> GetSelectListFromTermsUnderLevel(
            TaxonomyFieldViewModel viewModel, int startingLevel = 0, int depth = int.MaxValue)
        {
            var selectedTermName = viewModel.SelectedTerms.FirstOrDefault()?.Name;
            var selectListItems = new List<SelectListItem>();

            foreach (var entry in viewModel.Terms)
            {
                var entryLevel = entry.GetLevels();

                if (!entry.Selectable || entryLevel < startingLevel || (entryLevel - startingLevel) >= depth)
                    continue;

                selectListItems.Add(CreateSelectListItem(entry, selectedTermName, entryLevel - startingLevel));
            }

            return selectListItems;
        }

        public static Dictionary<string, Dictionary<string, string>> CreateValueHierarchyFromTerms(
            TaxonomyFieldViewModel viewModel, int parentLevel = 0, int depth = int.MaxValue)
        {
            var termEntries = viewModel.Terms;
            var topLevelTermEntries = termEntries.Where(term => term.GetLevels() == parentLevel).ToList();
            var valueHierarchy = new Dictionary<string, Dictionary<string, string>>();

            foreach (var entry in topLevelTermEntries)
            {
                var children = termEntries.Where(term => term.Path.Contains(entry.Path + entry.Id) && term.GetLevels() <= depth + parentLevel)
                    .ToDictionary(term => term.Id.ToString(), term => term.Name);
                valueHierarchy.Add(entry.Id.ToString(), children);
            }

            return valueHierarchy;
        }


        private static SelectListItem CreateSelectListItem(TermEntry term, string selectedTermName, int startingLevel)
        {
            var prefix = new StringBuilder();
            var indentation = "\xA0\xA0\xA0\xA0";
            for (int i = 0; i < (term.GetLevels() - startingLevel); i++) prefix.Append(indentation);

            return new SelectListItem
            {
                Text = prefix.ToString() + term.Name,
                Value = term.Id.ToString(),
                Selected = term.Name == selectedTermName
            };
        }
    }
}