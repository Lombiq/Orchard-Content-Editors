using OrchardCore.ContentManagement;

namespace Lombiq.ContentEditors.Samples.Models;

// Similarly to EmployeePart, this content part stores the information for the Support Ticket content items. However,
// it demonstrates that it isn't necessary to use content fields with async editors. You can have your custom content
// part shapes placed on async editor groups.
public class SupportTicketPart : ContentPart
{
    public string Name { get; set; }
    public string Email { get; set; }
    public string Url { get; set; }
    public string Description { get; set; }
}

// NEXT STATION: Drivers/SupportTicketPartDisplayDriver.cs
