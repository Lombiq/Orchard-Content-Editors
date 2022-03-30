using System.Collections.Generic;

namespace Lombiq.ContentEditors.Samples.Constants;

public static class EditorGroups
{
    public static class Employee
    {
        public const string PersonalDetails = nameof(PersonalDetails);
        public const string EmploymentDetails = nameof(EmploymentDetails);

        public static readonly IEnumerable<string> EditorGroups = new[]
        {
            PersonalDetails,
            EmploymentDetails,
        };
    }

    public static class SupportTicket
    {
        public const string Reporter = nameof(Reporter);
        public const string Details = nameof(Details);
        public const string Summary = nameof(Summary);

        public static readonly IEnumerable<string> EditorGroups = new[]
        {
            Reporter,
            Details,
            Summary,
        };
    }
}
