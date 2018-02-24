using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;
using System.Collections.Generic;

namespace Lombiq.EditorGroups.Models
{
    public class AsyncEditorPart : ContentPart
    {
        public bool AsyncEditorContext { get; set; }
        public EditorGroupDescriptor CurrentEditorGroup { get; set; }


        public string CompleteEditorGroupNamesSerialized
        {
            get { return this.Retrieve(x => x.CompleteEditorGroupNamesSerialized); }
            set { this.Store(x => x.CompleteEditorGroupNamesSerialized, value); }
        }

        private readonly LazyField<bool> _hasEditorGroups = new LazyField<bool>();
        internal LazyField<bool> HasEditorGroupsField => _hasEditorGroups;
        public bool HasEditorGroups => _hasEditorGroups.Value;

        private readonly LazyField<UnauthorizedEditorGroupBehavior> _unauthorizedEditorGroupBehavior = new LazyField<UnauthorizedEditorGroupBehavior>();
        internal LazyField<UnauthorizedEditorGroupBehavior> UnauthorizedEditorGroupBehaviorField => _unauthorizedEditorGroupBehavior;
        public UnauthorizedEditorGroupBehavior UnauthorizedEditorGroupBehavior => _unauthorizedEditorGroupBehavior.Value;

        private readonly LazyField<IEnumerable<EditorGroupDescriptor>> _editorGroups = new LazyField<IEnumerable<EditorGroupDescriptor>>();
        internal LazyField<IEnumerable<EditorGroupDescriptor>> EditorGroupsField => _editorGroups;
        public IEnumerable<EditorGroupDescriptor> EditorGroups => _editorGroups.Value;

        private readonly LazyField<IEnumerable<string>> _completeEditorGroupNames = new LazyField<IEnumerable<string>>();
        internal LazyField<IEnumerable<string>> CompleteEditorGroupNamesField => _completeEditorGroupNames;
        public IEnumerable<string> CompleteEditorGroupNames
        {
            get { return _completeEditorGroupNames.Value; }
            set { _completeEditorGroupNames.Value = value; }
        }

        private readonly LazyField<IEnumerable<string>> _incompleteEditorGroupNames = new LazyField<IEnumerable<string>>();
        internal LazyField<IEnumerable<string>> IncompleteEditorGroupNamesField => _incompleteEditorGroupNames;
        public IEnumerable<string> IncompleteEditorGroupNames => _incompleteEditorGroupNames.Value;

        private readonly LazyField<IEnumerable<EditorGroupDescriptor>> _authorizedEditorGroups = new LazyField<IEnumerable<EditorGroupDescriptor>>();
        internal LazyField<IEnumerable<EditorGroupDescriptor>> AuthorizedEditorGroupsField => _authorizedEditorGroups;
        public IEnumerable<EditorGroupDescriptor> AuthorizedEditorGroups => _authorizedEditorGroups.Value;

        private readonly LazyField<EditorGroupDescriptor> _nextEditorGroup = new LazyField<EditorGroupDescriptor>();
        internal LazyField<EditorGroupDescriptor> NextEditorGroupField => _nextEditorGroup;
        public EditorGroupDescriptor NextEditorGroup => _nextEditorGroup.Value;

        private readonly LazyField<EditorGroupDescriptor> _previousEditorGroup = new LazyField<EditorGroupDescriptor>();
        internal LazyField<EditorGroupDescriptor> PreviousEditorGroupField => _previousEditorGroup;
        public EditorGroupDescriptor PreviousEditorGroup => _previousEditorGroup.Value;
    }
}