using Lombiq.EditorGroups.Constants;
using Lombiq.EditorGroups.Models;
using Orchard.ContentManagement;
using Orchard.ContentManagement.Drivers;
using Orchard.ContentManagement.Handlers;

namespace Lombiq.EditorGroups.Drivers
{
    public class TestContentPartDriver : ContentPartDriver<TestContentPart>
    {
        private readonly IContentManager _contentManager;


        public TestContentPartDriver(IContentManager contentManager)
        {
            _contentManager = contentManager;
        }


        protected override DriverResult Editor(TestContentPart part, dynamic shapeHelper) =>
            ContentShape("Parts_TestContent_NewTestContentTypeWithGroups_Edit", () => 
                shapeHelper.Lombiq_AsyncEditorLoader(ContentItem: 
                    _contentManager.New(AsyncEditorTestConstants.TestContentTypeForNesting)));
    };
}