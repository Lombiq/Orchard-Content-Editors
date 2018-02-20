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
            var testContentItemEditorShape = _contentManager.BuildEditor(GetItem(id, EditorGroupsTestConstants.TestContentTypeName, "Cica"), "Cica");

            return View(testContentItemEditorShape);
        }
        


        private ContentItem GetItem(int id, string contentType, string editorGroup)
        {
            var item = _contentManager.Get(id);

            item = item ?? _contentManager.New(contentType);

            var part = item.As<EditorGroupsPart>();
            part.AsyncEditorContext = true;

            return item;
        }
    }
}