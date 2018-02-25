using Lombiq.EditorGroups.Constants;
using Lombiq.EditorGroups.Models;
using Orchard.ContentManagement;
using Orchard.Core.Contents;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Themes;
using System.Web.Mvc;

namespace Lombiq.EditorGroups.Controllers
{
    [Themed]
    public class AsyncEditorTestController : Controller
    {
        private readonly IContentManager _contentManager;
        private readonly dynamic _shapeFactory;


        public AsyncEditorTestController(IContentManager contentManager, IShapeFactory shapeFactory)
        {
            _contentManager = contentManager;
            _shapeFactory = shapeFactory;
        }


        public ActionResult TestWithGroups(int id = 0)
        {
            var testContentItemEditorShape = _contentManager.BuildEditor(GetItem(id, AsyncEditorTestConstants.TestContentTypeWithGroups));

            return View(testContentItemEditorShape);
        }

        public ActionResult TestWithoutGroups(int id = 0)
        {
            var item = GetItem(id, AsyncEditorTestConstants.TestContentTypeWithoutGroups);
            var loaderShape = _shapeFactory.Lombiq_AsyncEditorLoader(ContentItem: item);

            return View(loaderShape);
        }
        

        private ContentItem GetItem(int id, string contentType) =>
            id == 0 ?
                _contentManager.New(contentType) :
                _contentManager.Get(id, VersionOptions.Latest);
    }
}