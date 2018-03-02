using Lombiq.EditorGroups.Models;
using Lombiq.EditorGroups.Services;
using Orchard.ContentManagement.Handlers;

namespace Lombiq.EditorGroups.Handlers
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

                part.NextEditorGroupField.Loader(() =>
                    part.CurrentEditorGroup == null ?
                        null :
                        asyncEditorService.GetNextGroupDescriptor(part, part.CurrentEditorGroup.Name, true));

                part.PreviousEditorGroupField.Loader(() =>
                    part.CurrentEditorGroup == null ?
                        null :
                        asyncEditorService.GetPreviousGroupDescriptor(part, part.CurrentEditorGroup.Name, true));
            });
        }
    }
}