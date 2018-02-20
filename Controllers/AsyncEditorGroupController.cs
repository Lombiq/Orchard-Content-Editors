using Lombiq.EditorGroups.Constants;
using Lombiq.EditorGroups.Models;
using Lombiq.EditorGroups.Services;
using Orchard.ContentManagement;
using Orchard.Core.Contents;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Themes;
using System;
using System.Web.Mvc;

namespace Lombiq.EditorGroups.Controllers
{
    public class AsyncEditorGroupController : Controller
    {
        private readonly IContentManager _contentManager;
        private readonly IAuthorizer _authorizer;
        private readonly IShapeDisplay _shapeDisplay;
        private readonly dynamic _shapeFactory;
        private readonly IAsyncEditorService _asyncEditorService;


        public Localizer T { get; set; }


        public AsyncEditorGroupController(IContentManager contentManager, IAuthorizer authorizer, IShapeDisplay shapeDisplay, IShapeFactory shapeFactory, IAsyncEditorService asyncEditorService)
        {
            _contentManager = contentManager;
            _authorizer = authorizer;
            _shapeDisplay = shapeDisplay;
            _shapeFactory = shapeFactory;
            _asyncEditorService = asyncEditorService;

            T = NullLocalizer.Instance;
        }


        public ActionResult GetEditorGroup(int contentItemId, string group, string contentType = "")
        {
            if (string.IsNullOrEmpty(group)) return ErrorResult(T("Group name cannot be empty."));

            var contentItem = GetItem(contentItemId, contentType);

            if (!_asyncEditorService.IsAuthorizedToEditGroup(contentItem, group))
            {
                return ErrorResult(T("You don't have permission to edit this content item on this editor group."));
            }

            var editorShape = _contentManager.BuildEditor(contentItem, group);

            return EditorGroupResult(editorShape);
        }


        private EditorGroupsPart GetItem(int id, string contentType)
        {
            var item = _contentManager.Get(id);

            item = item ?? _contentManager.New(contentType);

            return item.As<EditorGroupsPart>();
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