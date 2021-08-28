﻿namespace Lombiq.ContentEditors.ViewModels
{
    public enum TaxonomyEditorFlavor
    {
        Default,
        Dropdown,
        Checkbox,
        RadioButton,
        Selectize
    }

    public class TaxonomyEditorViewModel : DropdownEditorViewModel
    {
        public Orchard.Taxonomies.ViewModels.TaxonomyFieldViewModel TaxonomyFieldViewModel { get; set; }
        public TaxonomyEditorFlavor Flavor { get; set; } = TaxonomyEditorFlavor.Default;

        public TaxonomyEditorViewModel()
        {
            DefaultEmptyValue = "0";
        }
    }
}