using Lombiq.ContentEditors.Samples.Constants;
using Lombiq.ContentEditors.Samples.Models;
using Lombiq.ContentEditors.Samples.ViewModels;
using OrchardCore.ContentManagement.Display.ContentDisplay;
using OrchardCore.ContentManagement.Display.Models;
using OrchardCore.DisplayManagement.ModelBinding;
using OrchardCore.DisplayManagement.Views;
using System.Threading.Tasks;

namespace Lombiq.ContentEditors.Samples.Drivers;

public class SupportTicketPartDisplayDriver : ContentPartDisplayDriver<SupportTicketPart>
{
    public override IDisplayResult Edit(SupportTicketPart part, BuildPartEditorContext context) =>
        Combine(
            Initialize<EditSupportTicketReporterViewModel>(GetEditorShapeType(EditorGroups.SupportTicket.Reporter), viewModel =>
            {
                viewModel.Name = part.Name;
                viewModel.Email = part.Email;
            }).OnGroup(EditorGroups.SupportTicket.Reporter).Location("Content"),
            Initialize<EditSupportTicketDetailsViewModel>(GetEditorShapeType(EditorGroups.SupportTicket.Details), viewModel =>
            {
                viewModel.Url = part.Url;
                viewModel.Description = part.Description;
            }).OnGroup(EditorGroups.SupportTicket.Details).Location("Content"));

    public override async Task<IDisplayResult> UpdateAsync(SupportTicketPart part, IUpdateModel updater, UpdatePartEditorContext context)
    {
        switch (context.GroupId)
        {
            case EditorGroups.SupportTicket.Reporter:
                var reporterViewModel = new EditSupportTicketReporterViewModel();
                await updater.TryUpdateModelAsync(reporterViewModel, Prefix);

                part.Name = reporterViewModel.Name;
                part.Email = reporterViewModel.Email;

                break;
            case EditorGroups.SupportTicket.Details:
                var detailsViewModel = new EditSupportTicketDetailsViewModel();
                await updater.TryUpdateModelAsync(detailsViewModel, Prefix);

                part.Url = detailsViewModel.Url;
                part.Description = detailsViewModel.Description;

                break;
            default:
                await EditAsync(part, context);
                break;
        }

        return await EditAsync(part, context);
    }

    private static string GetEditorShapeType(string group) => $"{nameof(SupportTicketPart)}_{group}_Edit";
}
