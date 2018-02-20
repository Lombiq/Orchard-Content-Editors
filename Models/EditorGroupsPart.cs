using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;
using System.Collections.Generic;

namespace Lombiq.EditorGroups.Models
{
    public class EditorGroupsPart : ContentPart
    {
        public bool AsyncEditorContext { get; set; }
        public EditorGroupDescriptor CurrentEditorGroup { get; set; }


        public string FilledEditorGroupNamesSerialized
        {
            get { return this.Retrieve(x => x.FilledEditorGroupNamesSerialized); }
            set { this.Store(x => x.FilledEditorGroupNamesSerialized, value); }
        }


        private readonly LazyField<IEnumerable<EditorGroupDescriptor>> _editorGroups = new LazyField<IEnumerable<EditorGroupDescriptor>>();
        internal LazyField<IEnumerable<EditorGroupDescriptor>> EditorGroupsField => _editorGroups;
        public IEnumerable<EditorGroupDescriptor> EditorGroups => _editorGroups.Value;

        private readonly LazyField<IEnumerable<string>> _filledEditorGroupNames = new LazyField<IEnumerable<string>>();
        internal LazyField<IEnumerable<string>> FilledEditorGroupNamesField => _filledEditorGroupNames;
        public IEnumerable<string> FilledEditorGroupNames => _filledEditorGroupNames.Value;

        private readonly LazyField<IEnumerable<EditorGroupDescriptor>> _authorizedEditorGroups = new LazyField<IEnumerable<EditorGroupDescriptor>>();
        internal LazyField<IEnumerable<EditorGroupDescriptor>> AuthorizedEditorGroupsField { get { return _authorizedEditorGroups; } }
        public IEnumerable<EditorGroupDescriptor> AuthorizedEditorGroups { get { return _authorizedEditorGroups.Value; } }
    }
}