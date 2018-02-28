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
                        new EditorGroupDescriptor { Name = "TestForTitle", Title = T("Title").Text },
                        //new EditorGroupDescriptor { Name = "TestForBody", Title = T("Body").Text },
                        //new EditorGroupDescriptor { Name = "TestForFields", Title = T("Fields").Text },
                        new EditorGroupDescriptor { Name = "TestForNested", Title = T("Nested").Text, IsPublishGroup = true },
                    },
                UnauthorizedEditorGroupBehavior = UnauthorizedEditorGroupBehavior.AllowEditingUntilFirstUnauthorizedGroup
            };
    }

    public class TestEditorGroupsForNestingProvider : IEditorGroupsProvider
    {
        public Localizer T { get; set; }


        public TestEditorGroupsForNestingProvider()
        {
            T = NullLocalizer.Instance;
        }


        public bool CanProvideEditorGroups(string contentType) => contentType == AsyncEditorTestConstants.TestContentTypeForNesting;

        public EditorGroupsSettings GetEditorGroupsSettings() =>
            new EditorGroupsSettings
            {
                EditorGroups = new[]
                    {
                        new EditorGroupDescriptor { Name = "TestForTitle", Title = T("Title").Text },
                        new EditorGroupDescriptor { Name = "TestForBody", Title = T("Body").Text, IsPublishGroup = true },
                    },
                UnauthorizedEditorGroupBehavior = UnauthorizedEditorGroupBehavior.AllowEditingUntilFirstUnauthorizedGroup
            };
    }
}