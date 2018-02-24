using Lombiq.EditorGroups.Models;
using Lombiq.EditorGroups.Services;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Mvc;
using Orchard.Security;
using System.Web.Mvc;

namespace Lombiq.EditorGroups.Controllers
{
    public class AsyncEditorController : Controller, IUpdateModel
    {
        private readonly IContentManager _contentManager;
        private readonly IAuthorizer _authorizer;
        private readonly IShapeDisplay _shapeDisplay;
        private readonly dynamic _shapeFactory;
        private readonly IAsyncEditorService _asyncEditorService;
        private readonly ITransactionManager _transactionManager;


        public Localizer T { get; set; }


        public AsyncEditorController(IContentManager contentManager, IAuthorizer authorizer, IShapeDisplay shapeDisplay, IShapeFactory shapeFactory, IAsyncEditorService asyncEditorService, ITransactionManager transactionManager)
        {
            _contentManager = contentManager;
            _authorizer = authorizer;
            _shapeDisplay = shapeDisplay;
            _shapeFactory = shapeFactory;
            _asyncEditorService = asyncEditorService;
            _transactionManager = transactionManager;

            T = NullLocalizer.Instance;
        }


        public ActionResult Edit(int contentItemId, string group = "", string contentType = "")
        {
            if (string.IsNullOrEmpty(group)) return GroupNameCannotBeEmptyResult();

            var part = GetAsyncEditorPart(contentItemId, contentType);

            if (part == null) return ContentItemNotFoundResult();

            if (part.HasEditorGroups && string.IsNullOrEmpty(group)) return GroupNameCannotBeEmptyResult();

            if (!_asyncEditorService.IsAuthorizedToEditGroup(part, group)) return UnauthorizedGroupEditorResult();

            if (!_asyncEditorService.EditorGroupAvailable(part, group)) return GroupUnavailableResult();

            return EditorGroupResult(part, group);
        }

        [HttpPost, ActionName(nameof(AsyncEditorController.Edit))]
        [FormValueRequired("submit.Save")]
        public ActionResult SavePost(int contentItemId, string group = "", string contentType = "") =>
            SaveDraftOrPublishPost(contentItemId, group, contentType, false);

        [HttpPost, ActionName(nameof(AsyncEditorController.Edit))]
        [FormValueRequired("submit.SaveAndNext")]
        public ActionResult SaveAndNextPost(int contentItemId, string group, string contentType = "")
        {
            if (string.IsNullOrEmpty(group)) return GroupNameCannotBeEmptyResult();

            var part = GetAsyncEditorPart(contentItemId, contentType);

            if (part == null || !part.HasEditorGroups) return ContentItemNotFoundResult();

            if (!_asyncEditorService.IsAuthorizedToEditGroup(part, group)) return UnauthorizedGroupEditorResult();

            if (!_asyncEditorService.EditorGroupAvailable(part, group)) return GroupUnavailableResult();

            _contentManager.Create(part.ContentItem, VersionOptions.Draft);
            _contentManager.UpdateEditor(part.ContentItem, this, group);

            if (!ModelState.IsValid)
            {
                _transactionManager.Cancel();

                return EditorGroupResult(part, group);
            }

            _asyncEditorService.StoreCompleteEditorGroup(part, group);

            var nextGroup = _asyncEditorService.GetNextAuthorizedGroupDescriptor(part, group);
            if (nextGroup == null) return EditorGroupResult(part, group);

            return EditorGroupResult(part, nextGroup.Name);
        }


        private AsyncEditorPart GetAsyncEditorPart(int id, string contentType) =>
            id == 0 ? 
                _contentManager.New<AsyncEditorPart>(contentType) :
                _contentManager.Get<AsyncEditorPart>(id, VersionOptions.Latest);

        private string GetEditorShapeHtml(AsyncEditorPart part, string group)
        {
            var editorShape = _asyncEditorService.BuildAsyncEditorShape(part, group);
            var formShape = _shapeFactory.AsyncEditor_Editor(
                ValidationSummaryShape: _shapeFactory.AsyncEditor_ValidationSummary(ModelState: ModelState),
                EditorShape: editorShape, 
                ContentItem: part.ContentItem,
                Group: group);

            return _shapeDisplay.Display(formShape);
        }

        private ActionResult ContentItemNotFoundResult() =>
            ErrorResult(T("Content item has not found."));

        private ActionResult GroupNameCannotBeEmptyResult() =>
            ErrorResult(T("Group name cannot be empty."));

        private ActionResult UnauthorizedGroupEditorResult() =>
            ErrorResult(T("You don't have permission to edit this content item on this editor group."));

        private ActionResult GroupUnavailableResult() =>
            ErrorResult(T("Editor group is not available. Fill all the previous editor groups first."));

        private ActionResult ErrorResult(LocalizedString errorMessage) =>
            Json(new AsyncEditorResult
            {
                Success = false,
                ErrorMessage = errorMessage.Text,
            }, JsonRequestBehavior.AllowGet);

        private ActionResult EditorGroupResult(AsyncEditorPart part, string group) =>
            Json(new AsyncEditorResult
            {
                Success = true,
                ContentItemId = part.ContentItem.Id,
                EditorShape = GetEditorShapeHtml(part, group),
                EditorGroup = group
            }, JsonRequestBehavior.AllowGet);
        
        private ActionResult SaveDraftOrPublishPost(int contentItemId, string group = "", string contentType = "", bool publish = false)
        {
            if (string.IsNullOrEmpty(group)) return GroupNameCannotBeEmptyResult();

            var part = GetAsyncEditorPart(contentItemId, contentType);

            if (part == null) return ContentItemNotFoundResult();

            if (part.HasEditorGroups && string.IsNullOrEmpty(group)) return GroupNameCannotBeEmptyResult();

            if (!_asyncEditorService.IsAuthorizedToEditGroup(part, group)) return UnauthorizedGroupEditorResult();

            if (!_asyncEditorService.EditorGroupAvailable(part, group)) return GroupUnavailableResult();

            if (part.HasEditorGroups)
            {
                var currentEditorGroupDescriptor = _asyncEditorService.GetEditorGroupDescriptor(part, group);
                if (publish && !currentEditorGroupDescriptor.PublishGroup)
                {
                    return ErrorResult(T("The current group is not a publish group. Editor hasn't been updated."));
                }
            }

            _contentManager.Create(part.ContentItem, VersionOptions.Draft);
            _contentManager.UpdateEditor(part.ContentItem, this, group);

            if (!ModelState.IsValid)
            {
                _transactionManager.Cancel();

                return EditorGroupResult(part, group);
            }

            if (part.HasEditorGroups)
            {
                _asyncEditorService.StoreCompleteEditorGroup(part, group);
            }

            if (publish)
            {
                _contentManager.Publish(part.ContentItem);
            }

            return EditorGroupResult(part, group);
        }


        #region IUpdateModel

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) =>
            TryUpdateModel(model, prefix, includeProperties, excludeProperties);

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) =>
            ModelState.AddModelError(key, errorMessage.ToString());

        #endregion IUpdateModel
    }
}