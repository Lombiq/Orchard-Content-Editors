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


        public bool CanProvideEditorGroups(string contentType) => contentType == EditorGroupsTestConstants.TestContentTypeName;

        public IEnumerable<EditorGroupDescriptor> GetEditorGroups() => 
            new[]
            {
                new EditorGroupDescriptor { Name = "TestGroup1", Title = T("Title").Text },
                new EditorGroupDescriptor { Name = "TestGroup2", Title = T("Content").Text },
                new EditorGroupDescriptor { Name = "TestGroup3", Title = T("Extra").Text },
            };
    }
}