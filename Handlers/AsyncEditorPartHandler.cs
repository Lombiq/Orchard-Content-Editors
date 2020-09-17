using Lombiq.ContentEditors.Models;
using Lombiq.ContentEditors.Services;
using Orchard.ContentManagement.Handlers;
using System.Linq;

namespace Lombiq.ContentEditors.Handlers
{
    public class AsyncEditorPartHandler : ContentHandler
    {
        public AsyncEditorPartHandler(
            IEditorGroupsProviderAccessor editorGroupsProviderAccessor,
            IAsyncEditorService asyncEditorService)
        {
            OnActivated<AsyncEditorPart>((context, part) =>
            {
                part.EditorGroupsSettingsField.Loader(() =>
                    editorGroupsProviderAccessor.GetProvider(part.TypeDefinition.Name)?.GetEditorGroupsSettings());

                part.HasEditorGroupsField.Loader(() => part.EditorGroupsSettings != null);

                part.UnauthorizedEditorGroupBehaviorField.Loader(() =>
                    part.EditorGroupsSettings?.UnauthorizedEditorGroupBehavior ??
                    UnauthorizedEditorGroupBehavior.AllowEditingAllAuthorizedGroups);

                part.AuthorizedEditorGroupsField.Loader(() => asyncEditorService.GetAuthorizedEditorGroups(part));

                part.CompletedAuthorizedEditorGroupsField.Loader(() =>
                    asyncEditorService.GetCompletedEditorGroups(part, true));

                part.IncompleteAuthorizedEditorGroupsField.Loader(() =>
                    asyncEditorService.GetIncompleteEditorGroups(part, true));

                part.AvailableAuthorizedEditorGroupsField.Loader(() =>
                    asyncEditorService.GetAvailableEditorGroups(part, true));

                part.NextAuthorizedEditorGroupField.Loader(() =>
                    part.CurrentEditorGroup == null ?
                        null :
                        asyncEditorService.GetNextGroupDescriptor(part, part.CurrentEditorGroup.Name, true));

                part.PreviousAuthorizedEditorGroupField.Loader(() =>
                    part.CurrentEditorGroup == null ?
                        null :
                        asyncEditorService.GetPreviousGroupDescriptor(part, part.CurrentEditorGroup.Name, true));

                part.NextEditableAuthorizedGroupField.Loader(() =>
                    asyncEditorService.GetIncompleteEditorGroups(part, true).FirstOrDefault() ??
                        asyncEditorService.GetCompletedEditorGroups(part, true).LastOrDefault());

                part.LastUpdatedEditorGroupField.Loader(() =>
                    !string.IsNullOrEmpty(part.LastUpdatedEditorGroupName) ?
                        asyncEditorService
                            .GetAuthorizedEditorGroups(part)
                            .FirstOrDefault(group => group.Name == part.LastUpdatedEditorGroupName) :
                        null);

                part.AreAllEditorGroupsCompletedField.Loader(() =>
                    !asyncEditorService.GetIncompleteEditorGroups(part).Any());

                part.LastDisplayedEditorGroupField.Loader(() =>
                    !string.IsNullOrEmpty(part.LastDisplayedEditorGroupName) ?
                        asyncEditorService
                            .GetAuthorizedEditorGroups(part)
                            .FirstOrDefault(group => group.Name == part.LastDisplayedEditorGroupName) :
                        null);
            });
        }
    }
}