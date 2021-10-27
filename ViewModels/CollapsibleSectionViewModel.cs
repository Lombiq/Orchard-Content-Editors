using System;

namespace Lombiq.ContentEditors.ViewModels
{
    public class CollapsibleSectionViewModel : FieldViewModel
    {
        public string DataTargetId { get; set; } = Guid.NewGuid().ToString("N");
        public string Body { get; set; }
        public dynamic BodyShape { get; set; }
    }
}
