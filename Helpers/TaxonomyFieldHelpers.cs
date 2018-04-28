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
        public static IList<TermEntry> GetTermEntryListFromTermsUnderParent(
            TaxonomyFieldViewModel viewModel, int? parentId = null, int depth = int.MaxValue)
        {
            var parentTermLevel = parentId == null ? 0 : viewModel.Terms.Where(term => term.Id == parentId).FirstOrDefault().GetLevels();
            var termEntries = new List<TermEntry>();

            foreach (var entry in viewModel.Terms)
            {
                if (!entry.Selectable ||
                    (parentId == null || !entry.Path.Contains("/" + parentId) || (entry.GetLevels() - parentTermLevel) > depth))
                    continue;

                termEntries.Add(entry);
            }

            return termEntries.OrderBy(entry => entry.Name).ToList();
        }

        public static IList<SelectListItem> GetSelectListFromTermsUnderParent(
            IList<TermEntry> termEntries, int levelOffset = 0, IEnumerable<string> selectedTermNames = null) =>
            termEntries
                .Select(entry => CreateSelectListItem(entry, selectedTermNames, entry.GetLevels() - levelOffset))
                .OrderBy(item => item.Text)
                .ToList();

        public static IList<SelectListItem> GetSelectListFromTermsUnderParent(
            TaxonomyFieldViewModel viewModel, int? parentId = null, int depth = int.MaxValue)
        {
            var selectedTermNames = viewModel.SelectedTerms?.Select(term => term.Name) ?? Enumerable.Empty<string>();
            var parentTermLevel = parentId == null ? 0 : viewModel.Terms.Where(term => term.Id == parentId).FirstOrDefault().GetLevels();

            return GetSelectListFromTermsUnderParent(GetTermEntryListFromTermsUnderParent(viewModel, parentId, depth), parentTermLevel, selectedTermNames);
        }

        public static IList<SelectListItem> GetSelectListFromTermsUnderLevel(
            TaxonomyFieldViewModel viewModel, int startingLevel = 0, int depth = int.MaxValue)
        {
            var selectedTermNames = viewModel.SelectedTerms?.Select(term => term.Name) ?? Enumerable.Empty<string>();
            var selectListItems = new List<SelectListItem>();

            foreach (var entry in viewModel.Terms)
            {
                var entryLevel = entry.GetLevels();

                if (!entry.Selectable || entryLevel < startingLevel || (entryLevel - startingLevel) >= depth)
                    continue;

                selectListItems.Add(CreateSelectListItem(entry, selectedTermNames, entryLevel - startingLevel));
            }

            return selectListItems.OrderBy(item => item.Text).ToList();
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


        private static SelectListItem CreateSelectListItem(TermEntry term, IEnumerable<string> selectedTermNames, int startingLevel)
        {
            var prefix = new StringBuilder();
            var indentation = "\xA0\xA0\xA0\xA0";
            for (int i = 0; i < (term.GetLevels() - startingLevel); i++) prefix.Append(indentation);

            return new SelectListItem
            {
                Text = prefix.ToString() + term.Name,
                Value = term.Id.ToString(),
                Selected = selectedTermNames != null && selectedTermNames.Contains(term.Name)
            };
        }
    }
}