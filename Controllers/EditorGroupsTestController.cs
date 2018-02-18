using Lombiq.EditorGroups.Constants;
using Orchard.ContentManagement;
using Orchard.Themes;
using System.Web.Mvc;

namespace Lombiq.EditorGroups.Controllers
{
    [Themed]
    public class EditorGroupsTestController : Controller
    {
        private readonly IContentManager _contentManager;


        public EditorGroupsTestController(IContentManager contentManager)
        {
            _contentManager = contentManager;
        }


        public ActionResult Index(int id = 0)
        {
            var testContentItemEditorShape = _contentManager.BuildEditor(_contentManager.New(EditorGroupsTestConstants.TestContentTypeName));

            return View(testContentItemEditorShape);
        }


        private ContentItem GetItem(int id)
        {
            var item = _contentManager.Get(id);

            if (item == null) return _contentManager.New(EditorGroupsTestConstants.TestContentTypeName);

            return item;
        }
    }
}