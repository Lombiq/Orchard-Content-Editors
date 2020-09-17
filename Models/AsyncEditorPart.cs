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


        internal LazyField<EditorGroupsSettings> EditorGroupsSettingsField { get; } = new LazyField<EditorGroupsSettings>();
        public EditorGroupsSettings EditorGroupsSettings => EditorGroupsSettingsField.Value;

        internal LazyField<bool> HasEditorGroupsField { get; } = new LazyField<bool>();
        public bool HasEditorGroups => HasEditorGroupsField.Value;

        internal LazyField<UnauthorizedEditorGroupBehavior> UnauthorizedEditorGroupBehaviorField { get; } = new LazyField<UnauthorizedEditorGroupBehavior>();
        public UnauthorizedEditorGroupBehavior UnauthorizedEditorGroupBehavior => UnauthorizedEditorGroupBehaviorField.Value;

        internal LazyField<IEnumerable<EditorGroupDescriptor>> CompletedAuthorizedEditorGroupsField { get; } = new LazyField<IEnumerable<EditorGroupDescriptor>>();
        public IEnumerable<EditorGroupDescriptor> CompletedAuthorizedEditorGroups => CompletedAuthorizedEditorGroupsField.Value;

        internal LazyField<IEnumerable<EditorGroupDescriptor>> IncompleteAuthorizedEditorGroupsField { get; } = new LazyField<IEnumerable<EditorGroupDescriptor>>();
        public IEnumerable<EditorGroupDescriptor> IncompleteAuthorizedEditorGroups => IncompleteAuthorizedEditorGroupsField.Value;

        internal LazyField<IEnumerable<EditorGroupDescriptor>> AuthorizedEditorGroupsField { get; } = new LazyField<IEnumerable<EditorGroupDescriptor>>();
        public IEnumerable<EditorGroupDescriptor> AuthorizedEditorGroups => AuthorizedEditorGroupsField.Value;

        internal LazyField<IEnumerable<EditorGroupDescriptor>> AvailableAuthorizedEditorGroupsField { get; } = new LazyField<IEnumerable<EditorGroupDescriptor>>();
        public IEnumerable<EditorGroupDescriptor> AvailableAuthorizedEditorGroups => AvailableAuthorizedEditorGroupsField.Value;

        internal LazyField<EditorGroupDescriptor> NextAuthorizedEditorGroupField { get; } = new LazyField<EditorGroupDescriptor>();
        public EditorGroupDescriptor NextAuthorizedEditorGroup => NextAuthorizedEditorGroupField.Value;

        internal LazyField<EditorGroupDescriptor> PreviousAuthorizedEditorGroupField { get; } = new LazyField<EditorGroupDescriptor>();
        public EditorGroupDescriptor PreviousAuthorizedEditorGroup => PreviousAuthorizedEditorGroupField.Value;

        internal LazyField<EditorGroupDescriptor> NextEditableAuthorizedGroupField { get; } = new LazyField<EditorGroupDescriptor>();
        public EditorGroupDescriptor NextEditableAuthorizedGroup => NextEditableAuthorizedGroupField.Value;

        internal LazyField<EditorGroupDescriptor> LastUpdatedEditorGroupField { get; } = new LazyField<EditorGroupDescriptor>();
        public EditorGroupDescriptor LastUpdatedEditorGroup => LastUpdatedEditorGroupField.Value;

        internal LazyField<bool> AreAllEditorGroupsCompletedField { get; } = new LazyField<bool>();
        public bool AreAllEditorGroupsCompleted => AreAllEditorGroupsCompletedField.Value;

        internal LazyField<EditorGroupDescriptor> LastDisplayedEditorGroupField { get; } = new LazyField<EditorGroupDescriptor>();
        public EditorGroupDescriptor LastDisplayedEditorGroup => LastDisplayedEditorGroupField.Value;
    }
}