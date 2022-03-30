using OrchardCore.ContentManagement;

namespace Lombiq.ContentEditors.Samples.Models;

public class SupportTicketPart : ContentPart
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Url { get; set; }
    public string Description { get; set; }
}
