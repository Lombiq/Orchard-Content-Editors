using System.Collections.Generic;

namespace Lombiq.EditorGroups.Models
{
    public enum UnauthorizedEditorGroupBehavior
    {
        AllowEditingAllAuthorizedGroup,
        AllowEditingUntilFirstUnauthorizedGroup
    }


    public class EditorGroupsSettings
    {
        public IEnumerable<EditorGroupDescriptor> EditorGroups { get; set; }
        public UnauthorizedEditorGroupBehavior UnauthorizedEditorGroupBehavior { get; set; }
    }
}