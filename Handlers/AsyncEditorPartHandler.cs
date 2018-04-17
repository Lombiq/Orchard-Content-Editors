using Lombiq.ContentEditors.Models;
using Lombiq.ContentEditors.Services;
using Orchard.ContentManagement.Handlers;
using System.Linq;

namespace Lombiq.ContentEditors.Handlers
{
    public class AsyncEditorPartHandler : ContentHandler
    {
        public AsyncEditorPartHandler(IAsyncEditorService asyncEditorService)
        {
            OnActivated<AsyncEditorPart>((context, part) =>
            {
                part.HasEditorGroupsField.Loader(() => 
                    asyncEditorService.GetEditorGroupsSettings(part) != null);

                part.UnauthorizedEditorGroupBehaviorField.Loader(() => 
                    asyncEditorService.GetEditorGroupsSettings(part)?.UnauthorizedEditorGroupBehavior ?? 
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

                part.IsAllEditorGroupsCompletedField.Loader(() => 
                    !asyncEditorService.GetIncompleteEditorGroups(part).Any());
            });
        }
    }
}