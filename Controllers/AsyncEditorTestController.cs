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
    public class AsyncEditorTestController : Controller
    {
        private readonly IContentManager _contentManager;
        private readonly IAuthorizer _authorizer;


        public AsyncEditorTestController(IContentManager contentManager, IAuthorizer authorizer)
        {
            _contentManager = contentManager;
            _authorizer = authorizer;
        }


        public ActionResult TestWithGroups(int id = 0)
        {
            var testContentItemEditorShape = _contentManager.BuildEditor(GetItem(id, AsyncEditorTestConstants.TestContentTypeWithGroups));

            return View(testContentItemEditorShape);
        }

        public ActionResult TestWithoutGroups(int id = 0)
        {
            var testContentItemEditorShape = _contentManager.BuildEditor(GetItem(id, AsyncEditorTestConstants.TestContentTypeWithoutGroups));

            return View(testContentItemEditorShape);
        }
        

        private ContentItem GetItem(int id, string contentType) =>
            id == 0 ?
                _contentManager.New(contentType) :
                _contentManager.Get(id, VersionOptions.Latest);
    }
}