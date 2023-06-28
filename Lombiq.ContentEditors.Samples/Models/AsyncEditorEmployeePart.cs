using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;

namespace Lombiq.ContentEditors.Samples.Models;

// This is the part that holds the content fields of an employee that you want to show in your async editor. For this
// demonstration we are storing the values in content fields so that we don't need to create a content driver and shapes
// with view models as those are already taken care of by Orchard Core. Alternatively, you can use properties and editor
// shapes with their own view models; see the other example for the front-end async editor to see how to do that.
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
// NEXT STATION: Migrations/EmployeeMigrations.cs
