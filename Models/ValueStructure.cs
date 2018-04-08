using System.Collections.Generic;

namespace Lombiq.ContentEditors.Models
{
    public class ValueStructure
    {
        public string RootValue { get; set; }
        public IEnumerable<ValueNamePair> Children { get; set; }
    }

    public class ValueNamePair
    {
        public string Value { get; set; }
        public string Name { get; set; }
    }
}