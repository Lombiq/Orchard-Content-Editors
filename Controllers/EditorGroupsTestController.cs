using Lombiq.EditorGroups.Constants;
using Lombiq.EditorGroups.Models;
using Orchard.ContentManagement;
using Orchard.Core.Contents;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Themes;
using System.Web.Mvc;

namespace Lombiq.EditorGroups.Controllers
{
    [Themed]
    public class EditorGroupsTestController : Controller
    {
        private readonly IContentManager _contentManager;
        private readonly IAuthorizer _authorizer;


        public EditorGroupsTestController(IContentManager contentManager, IAuthorizer authorizer)
        {
            _contentManager = contentManager;
            _authorizer = authorizer;
        }


        public ActionResult Index(int id = 0)
        {
            var testContentItemEditorShape = _contentManager.BuildEditor(_contentManager.New(EditorGroupsTestConstants.TestContentTypeName), "Cica");

            return View(testContentItemEditorShape);
        }
        


        private ContentItem GetItem(int id, string contentType)
        {
            var item = _contentManager.Get(id);

            if (item == null) return _contentManager.New(contentType);

            return item;
        }
    }
}