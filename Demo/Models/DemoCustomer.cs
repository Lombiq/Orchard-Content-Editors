using OrchardCore.ContentFields.Fields;
using OrchardCore.ContentManagement;
using OrchardCore.Media.Fields;

namespace Lombiq.ContentEditors.Demo.Models
{
    public class DemoCustomer : ContentPart
    {
        public TextField FirstName { get; set; }
        public TextField LastName { get; set; }
        public DateField DateOfBirth { get; set; }
        public MediaField ProfilePicture { get; set; }
        public MediaField Documents { get; set; }
        public TextField AdditionalNotes { get; set; }
    }
}
