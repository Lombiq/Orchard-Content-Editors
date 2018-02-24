using Lombiq.EditorGroups.Constants;
using Lombiq.EditorGroups.Models;
using Orchard.Localization;
using System.Collections.Generic;

namespace Lombiq.EditorGroups.Services
{
    public class TestEditorGroupsProvider : IEditorGroupsProvider
    {
        public Localizer T { get; set; }


        public TestEditorGroupsProvider()
        {
            T = NullLocalizer.Instance;
        }


        public bool CanProvideEditorGroups(string contentType) => contentType == AsyncEditorTestConstants.TestContentTypeWithGroups;

        public EditorGroupsSettings GetEditorGroupsSettings() =>
            new EditorGroupsSettings
            {
                EditorGroups = new[]
                    {
                        new EditorGroupDescriptor { Name = "TestGroup1", Title = T("Title").Text },
                        new EditorGroupDescriptor { Name = "TestGroup2", Title = T("Content").Text },
                        new EditorGroupDescriptor { Name = "TestGroup3", Title = T("Extra").Text, PublishGroup = true },
                    },
                UnauthorizedEditorGroupBehavior = UnauthorizedEditorGroupBehavior.AllowEditingUntilFirstUnauthorizedGroup
            };
    }
}