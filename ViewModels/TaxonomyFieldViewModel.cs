using Orchard.Taxonomies.Models;
using System.Collections.Generic;

namespace Lombiq.ContentEditors.ViewModels
{
    public class TaxonomyFieldViewModel : FieldViewModel
    {
        public IEnumerable<TermPart> Terms { get; set; }
        public bool IsLink { get; set; }
    }
}