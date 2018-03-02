using System.Collections.Generic;

namespace Lombiq.EditorGroups.Models
{
    public enum UnauthorizedEditorGroupBehavior
    {
        AllowEditingAllAuthorizedGroups,
        AllowEditingUntilFirstUnauthorizedGroup
    }


    public class EditorGroupsSettings
    {
        public IEnumerable<EditorGroupDescriptor> EditorGroups { get; set; }
        public UnauthorizedEditorGroupBehavior UnauthorizedEditorGroupBehavior { get; set; }
    }
}