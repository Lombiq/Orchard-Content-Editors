using System.Collections.Generic;

namespace Lombiq.ContentEditors.Demo.Constants
{
    public static class EditorGroups
    {
        public static class DemoCustomer
        {
            public const string PersonalDetails = nameof(PersonalDetails);
            public const string AdditionalNotes = nameof(AdditionalNotes);

            public static readonly IEnumerable<string> EditorGroups = new[]
            {
                PersonalDetails,
                AdditionalNotes,
            };
        }
    }
}
