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
    public class AsyncGroupEditorController : Controller
    {
        private readonly IContentManager _contentManager;
        private readonly IAuthorizer _authorizer;
        private readonly IShapeDisplay _shapeDisplay;


        public Localizer T { get; set; }


        public AsyncGroupEditorController(IContentManager contentManager, IAuthorizer authorizer, IShapeDisplay shapeDisplay)
        {
            _contentManager = contentManager;
            _authorizer = authorizer;
            _shapeDisplay = shapeDisplay;

            T = NullLocalizer.Instance;
        }


        public ActionResult GetEditorGroup(int contentItemId, string group, string contentType = "")
        {
            if (string.IsNullOrEmpty(group)) return ErrorResult(T("Group name cannot be empty."));

            var contentItem = GetItem(contentItemId, contentType);

            if (!_authorizer.Authorize(Permissions.EditContent, contentItem))
            {
                return ErrorResult(T("You don't have permission to edit this content item."));
            }

            var editorShape = _contentManager.BuildEditor(contentItem, group);
            return null;
        }


        private ContentItem GetItem(int id, string contentType)
        {
            var item = _contentManager.Get(id);

            if (item == null) return _contentManager.New(contentType);

            return item;
        }

        private ActionResult ErrorResult(LocalizedString errorMessage) =>
            Json(new EditorGroupsResult
            {
                Success = false,
                ErrorMessage = errorMessage.Text,
            });

        private ActionResult EditorGroupResult(string editorShape) =>
            Json(new EditorGroupsResult
            {
                Success = true,
                EditorShape = editorShape,
            });
    }
}