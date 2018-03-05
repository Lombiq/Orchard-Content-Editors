using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;
using System.Collections.Generic;

namespace Lombiq.ContentEditors.Models
{
    public class AsyncEditorPart : ContentPart
    {
        public bool IsAsyncEditorContext { get; set; }
        public EditorGroupDescriptor CurrentEditorGroup { get; set; }


        public string CompletedEditorGroupNamesSerialized
        {
            get { return this.Retrieve(x => x.CompletedEditorGroupNamesSerialized); }
            set { this.Store(x => x.CompletedEditorGroupNamesSerialized, value); }
        }

        private readonly LazyField<bool> _hasEditorGroups = new LazyField<bool>();
        internal LazyField<bool> HasEditorGroupsField => _hasEditorGroups;
        public bool HasEditorGroups => _hasEditorGroups.Value;

        private readonly LazyField<UnauthorizedEditorGroupBehavior> _unauthorizedEditorGroupBehavior = new LazyField<UnauthorizedEditorGroupBehavior>();
        internal LazyField<UnauthorizedEditorGroupBehavior> UnauthorizedEditorGroupBehaviorField => _unauthorizedEditorGroupBehavior;
        public UnauthorizedEditorGroupBehavior UnauthorizedEditorGroupBehavior => _unauthorizedEditorGroupBehavior.Value;

        private readonly LazyField<IEnumerable<EditorGroupDescriptor>> _completedAuthorizedEditorGroups = new LazyField<IEnumerable<EditorGroupDescriptor>>();
        internal LazyField<IEnumerable<EditorGroupDescriptor>> CompletedAuthorizedEditorGroupsField => _completedAuthorizedEditorGroups;
        public IEnumerable<EditorGroupDescriptor> CompletedAuthorizedEditorGroups => _completedAuthorizedEditorGroups.Value;

        private readonly LazyField<IEnumerable<EditorGroupDescriptor>> _incompleteAuthorizedEditorGroups = new LazyField<IEnumerable<EditorGroupDescriptor>>();
        internal LazyField<IEnumerable<EditorGroupDescriptor>> IncompleteAuthorizedEditorGroupsField => _incompleteAuthorizedEditorGroups;
        public IEnumerable<EditorGroupDescriptor> IncompleteAuthorizedEditorGroups => _incompleteAuthorizedEditorGroups.Value;

        private readonly LazyField<IEnumerable<EditorGroupDescriptor>> _authorizedEditorGroups = new LazyField<IEnumerable<EditorGroupDescriptor>>();
        internal LazyField<IEnumerable<EditorGroupDescriptor>> AuthorizedEditorGroupsField => _authorizedEditorGroups;
        public IEnumerable<EditorGroupDescriptor> AuthorizedEditorGroups => _authorizedEditorGroups.Value;

        private readonly LazyField<IEnumerable<EditorGroupDescriptor>> _availableAuthorizedEditorGroups = new LazyField<IEnumerable<EditorGroupDescriptor>>();
        internal LazyField<IEnumerable<EditorGroupDescriptor>> AvailableAuthorizedEditorGroupsField => _availableAuthorizedEditorGroups;
        public IEnumerable<EditorGroupDescriptor> AvailableAuthorizedEditorGroups => _availableAuthorizedEditorGroups.Value;

        private readonly LazyField<EditorGroupDescriptor> _nextAuthorizedEditorGroup = new LazyField<EditorGroupDescriptor>();
        internal LazyField<EditorGroupDescriptor> NextEditorGroupField => _nextAuthorizedEditorGroup;
        public EditorGroupDescriptor NextAuthorizedEditorGroup => _nextAuthorizedEditorGroup.Value;

        private readonly LazyField<EditorGroupDescriptor> _previousAuthorizedEditorGroup = new LazyField<EditorGroupDescriptor>();
        internal LazyField<EditorGroupDescriptor> PreviousEditorGroupField => _previousAuthorizedEditorGroup;
        public EditorGroupDescriptor PreviousAuthorizedEditorGroup => _previousAuthorizedEditorGroup.Value;
    }
}