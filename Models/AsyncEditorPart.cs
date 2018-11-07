using Orchard.ContentManagement;
using Orchard.ContentManagement.Utilities;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace Lombiq.ContentEditors.Models
{
    public class AsyncEditorPart : ContentPart
    {
        public bool IsAsyncEditorContext { get; set; }
        public EditorGroupDescriptor CurrentEditorGroup { get; set; }
        public bool IsContentCreationFailed { get; set; }


        public string CompletedEditorGroupNamesSerialized
        {
            get { return this.Retrieve(x => x.CompletedEditorGroupNamesSerialized); }
            set { this.Store(x => x.CompletedEditorGroupNamesSerialized, value); }
        }

        public string LastUpdatedEditorGroupName
        {
            get { return this.Retrieve(x => x.LastUpdatedEditorGroupName); }
            set { this.Store(x => x.LastUpdatedEditorGroupName, value); }
        }

        public string LastDisplayedEditorGroupName
        {
            get { return this.Retrieve(x => x.LastDisplayedEditorGroupName); }
            set { this.Store(x => x.LastDisplayedEditorGroupName, value); }
        }

        public string EditorSessionSalt
        {
            get => this.Retrieve(x => x.EditorSessionSalt, () =>
            {
                // Generating a random cryptographically secure salt for generating content item-specific
                // tokens for cookies that will identify the editor session if anonymous users have permission
                // to edit the content item.
                var saltBytes = new byte[0x10];
                using (var cryptoService = new RNGCryptoServiceProvider()) cryptoService.GetBytes(saltBytes);

                var salt = Convert.ToBase64String(saltBytes);
                EditorSessionSalt = salt;

                return salt;
            });
            set => this.Store(x => x.EditorSessionSalt, value);
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
        internal LazyField<EditorGroupDescriptor> NextAuthorizedEditorGroupField => _nextAuthorizedEditorGroup;
        public EditorGroupDescriptor NextAuthorizedEditorGroup => _nextAuthorizedEditorGroup.Value;

        private readonly LazyField<EditorGroupDescriptor> _previousAuthorizedEditorGroup = new LazyField<EditorGroupDescriptor>();
        internal LazyField<EditorGroupDescriptor> PreviousAuthorizedEditorGroupField => _previousAuthorizedEditorGroup;
        public EditorGroupDescriptor PreviousAuthorizedEditorGroup => _previousAuthorizedEditorGroup.Value;

        private readonly LazyField<EditorGroupDescriptor> _nextEditableAuthorizedGroup = new LazyField<EditorGroupDescriptor>();
        internal LazyField<EditorGroupDescriptor> NextEditableAuthorizedGroupField => _nextEditableAuthorizedGroup;
        public EditorGroupDescriptor NextEditableAuthorizedGroup => _nextEditableAuthorizedGroup.Value;

        private readonly LazyField<EditorGroupDescriptor> _lastUpdatedEditorGroup = new LazyField<EditorGroupDescriptor>();
        internal LazyField<EditorGroupDescriptor> LastUpdatedEditorGroupField => _lastUpdatedEditorGroup;
        public EditorGroupDescriptor LastUpdatedEditorGroup => _lastUpdatedEditorGroup.Value;

        private readonly LazyField<bool> _isAllEditorGroupsCompleted = new LazyField<bool>();
        internal LazyField<bool> IsAllEditorGroupsCompletedField => _isAllEditorGroupsCompleted;
        public bool IsAllEditorGroupsCompleted => _isAllEditorGroupsCompleted.Value;

        private readonly LazyField<EditorGroupDescriptor> _lastDisplayedEditorGroup = new LazyField<EditorGroupDescriptor>();
        internal LazyField<EditorGroupDescriptor> LastDisplayedEditorGroupField => _lastDisplayedEditorGroup;
        public EditorGroupDescriptor LastDisplayedEditorGroup => _lastDisplayedEditorGroup.Value;
    }
}