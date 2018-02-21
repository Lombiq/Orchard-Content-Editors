using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;
using System.Collections.Generic;

namespace Lombiq.EditorGroups.Models
{
    public class EditorGroupsPart : ContentPart
    {
        public bool AsyncEditorContext { get; set; }
        public EditorGroupDescriptor CurrentEditorGroup { get; set; }


        public string CompleteEditorGroupNamesSerialized
        {
            get { return this.Retrieve(x => x.CompleteEditorGroupNamesSerialized); }
            set { this.Store(x => x.CompleteEditorGroupNamesSerialized, value); }
        }


        private readonly LazyField<UnauthorizedEditorGroupBehavior> _unauthorizedEditorGroupBehavior = new LazyField<UnauthorizedEditorGroupBehavior>();
        internal LazyField<UnauthorizedEditorGroupBehavior> UnauthorizedEditorGroupBehaviorField => _unauthorizedEditorGroupBehavior;
        public UnauthorizedEditorGroupBehavior UnauthorizedEditorGroupBehavior => _unauthorizedEditorGroupBehavior.Value;

        private readonly LazyField<IEnumerable<EditorGroupDescriptor>> _editorGroups = new LazyField<IEnumerable<EditorGroupDescriptor>>();
        internal LazyField<IEnumerable<EditorGroupDescriptor>> EditorGroupsField => _editorGroups;
        public IEnumerable<EditorGroupDescriptor> EditorGroups => _editorGroups.Value;

        private readonly LazyField<IEnumerable<string>> _completeEditorGroupNames = new LazyField<IEnumerable<string>>();
        internal LazyField<IEnumerable<string>> CompleteEditorGroupNamesField => _completeEditorGroupNames;
        public IEnumerable<string> CompleteEditorGroupNames => _completeEditorGroupNames.Value;

        private readonly LazyField<IEnumerable<string>> _incompleteEditorGroupNames = new LazyField<IEnumerable<string>>();
        internal LazyField<IEnumerable<string>> IncompleteEditorGroupNamesField => _incompleteEditorGroupNames;
        public IEnumerable<string> IncompleteEditorGroupNames => _incompleteEditorGroupNames.Value;

        private readonly LazyField<IEnumerable<EditorGroupDescriptor>> _authorizedEditorGroups = new LazyField<IEnumerable<EditorGroupDescriptor>>();
        internal LazyField<IEnumerable<EditorGroupDescriptor>> AuthorizedEditorGroupsField => _authorizedEditorGroups;
        public IEnumerable<EditorGroupDescriptor> AuthorizedEditorGroups => _authorizedEditorGroups.Value;
    }
}