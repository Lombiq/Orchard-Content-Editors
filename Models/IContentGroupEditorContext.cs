using Orchard.ContentManagement;
using System.Collections.Generic;

namespace Lombiq.EditorGroups.Models
{
    public interface IContentGroupEditorContext : IContent
    {
        bool AsyncEditorContext { get; set; }
        EditorGroupDescriptor CurrentEditorGroup { get; set; }
        IEnumerable<EditorGroupDescriptor> EditorGroups { get; }
        IEnumerable<string> FilledEditorGroupNames { get; }
    }
}