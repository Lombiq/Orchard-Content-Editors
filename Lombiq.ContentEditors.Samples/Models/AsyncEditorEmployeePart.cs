using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;

namespace Lombiq.ContentEditors.Samples.Models;

// This is the part that holds the content fields you want to show in your async editor.
public class AsyncEditorEmployeePart : ContentPart
{
    public TextField Name { get; set; } = new();
    public TextField Position { get; set; } = new();
    public TextField Office { get; set; } = new();
    public NumericField Age { get; set; } = new();
    public DateField StartDate { get; set; } = new();
    public NumericField Salary { get; set; } = new();
}

// NEXT STATION: placement.json -- Here observe how each content field is placed in the supported editor groups, then
// come back.
// END OF TRAINING SECTION: Content item async editors.
