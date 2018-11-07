using System.Collections.Generic;
using System.Linq;

namespace Lombiq.ContentEditors.ViewModels
{
    public class SingleChoiceEditorViewModel : EditorViewModel
    {
        public IEnumerable<string> Options { get; set; } = Enumerable.Empty<string>();
        public string OtherName { get; set; }
        public string OtherValue { get; set; }
        public bool AllowMultipleChoices { get; set; } = true;
        public string MultipleChoiceSeparator { get; set; } = ",";
    }
}