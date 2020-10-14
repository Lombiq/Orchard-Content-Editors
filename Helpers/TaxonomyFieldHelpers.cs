using Orchard.Taxonomies.Helpers;
using Orchard.Taxonomies.Models;
using Orchard.Taxonomies.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.Mvc;

namespace Lombiq.ContentEditors.Helpers
{
    public static class TaxonomyFieldHelpers
    {
        public static List<TermEntry> GetTermEntryListFromTermsUnderParent(
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

            return termEntries.OrderBy(term => term.Weight).ThenBy(term => term.Name).ToList();
        }

        public static List<SelectListItem> GetSelectListFromTermsUnderParent(
            List<TermEntry> termEntries, int levelOffset = 0, IEnumerable<string> selectedTermNames = null) =>
            termEntries
                .OrderBy(term => term.Weight).ThenBy(term => term.Name)
                .Select(entry => CreateSelectListItem(entry, selectedTermNames, entry.GetLevels() - levelOffset))
                .ToList();

        public static List<SelectListItem> GetSelectListFromTermsUnderParent(
            TaxonomyFieldViewModel viewModel, int? parentId = null, int depth = int.MaxValue)
        {
            var selectedTermNames = viewModel.SelectedTerms?.Select(term => term.Name) ?? Enumerable.Empty<string>();
            var parentTermLevel = parentId == null ? 0 : viewModel.Terms.Where(term => term.Id == parentId).FirstOrDefault().GetLevels();

            return GetSelectListFromTermsUnderParent(GetTermEntryListFromTermsUnderParent(viewModel, parentId, depth), parentTermLevel, selectedTermNames);
        }

        public static List<SelectListItem> GetSelectListFromTermsUnderLevel(
            TaxonomyFieldViewModel viewModel, int startingLevel = 0, int depth = int.MaxValue)
        {
            if (viewModel.SelectedTerms == null) viewModel.SelectedTerms = Enumerable.Empty<TermPart>();
            var selectListItems = new List<SelectListItem>();

            foreach (var entry in viewModel.Terms.OrderBy(term => term.Weight).ThenBy(term => term.Name))
            {
                var entryLevel = entry.GetLevels();

                // Selected Terms should still be returned even if they are no longer selectable to avoid loss of information.
                if ((!entry.Selectable && !viewModel.SelectedTerms.Select(term => term.Id).Contains(entry.Id)) ||
                    entryLevel < startingLevel ||
                    (entryLevel - startingLevel) >= depth)
                    continue;

                selectListItems.Add(CreateSelectListItem(entry, viewModel.SelectedTerms.Select(term => term.Name), entryLevel - startingLevel));
            }

            return selectListItems.ToList();
        }

        public static Dictionary<string, IEnumerable<string>> CreateValueHierarchyFromTerms(
            IList<TermEntry> terms, int parentLevel = 0)
        {
            var topLevelTermEntries = terms.Where(term => term.GetLevels() == parentLevel).ToList();
            var valueHierarchy = new Dictionary<string, IEnumerable<string>>();

            foreach (var entry in topLevelTermEntries)
            {
                var children = terms.Where(term => term.Path == $"{entry.Path}{entry.Id}/").Select(term => term.Id.ToString());
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