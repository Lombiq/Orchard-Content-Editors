using System.Collections.Generic;

namespace Lombiq.ContentEditors.ViewModels
{
    interface IParentElementValueDependency
    {
        string ParentElementSelector { get; set; }
        Dictionary<string, IEnumerable<string>> ParentElementValueHierarchy { get; set; }
    }
}
