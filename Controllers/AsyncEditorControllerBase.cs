using Lombiq.ContentEditors.Models;
using Lombiq.ContentEditors.Services;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Mvc;
using System.Web.Mvc;

namespace Lombiq.ContentEditors.Controllers
{
    public abstract class AsyncEditorControllerBase : Controller, IUpdateModel
    {
        protected readonly IContentManager _contentManager;
        protected readonly IShapeDisplay _shapeDisplay;
        protected readonly dynamic _shapeFactory;
        protected readonly IAsyncEditorService _asyncEditorService;
        protected readonly ITransactionManager _transactionManager;


        public Localizer T { get; set; }


        public AsyncEditorControllerBase(
            IContentManager contentManager,
            IShapeDisplay shapeDisplay,
            IShapeFactory shapeFactory,
            IAsyncEditorService asyncEditorService,
            ITransactionManager transactionManager)
        {
            _contentManager = contentManager;
            _shapeDisplay = shapeDisplay;
            _shapeFactory = shapeFactory;
            _asyncEditorService = asyncEditorService;
            _transactionManager = transactionManager;

            T = NullLocalizer.Instance;
        }


        #region Helpers
        
        protected virtual ActionResult EditResult(AsyncEditorPart part, string group = "")
        {
            if (part == null) return ContentItemNotFoundResult();

            if (part.HasEditorGroups && string.IsNullOrEmpty(group)) return GroupNameCannotBeEmptyResult();

            if (!_asyncEditorService.IsAuthorizedToEdit(part, group)) return UnauthorizedGroupEditorResult();

            if (!string.IsNullOrEmpty(group) &&
                !_asyncEditorService.IsEditorGroupAvailable(part, group)) return GroupUnavailableResult();

            return AsyncEditorResult(part, group);
        }

        protected virtual ActionResult EditResult(int contentItemId, string group = "", string contentType = "") =>
            EditResult(GetAsyncEditorPart(contentItemId, contentType));

        protected virtual ActionResult SaveDraftOrPublishResult(AsyncEditorPart part, string group = "", bool publish = false)
        {
            if (part == null) return ContentItemNotFoundResult();

            if (part.HasEditorGroups && string.IsNullOrEmpty(group)) return GroupNameCannotBeEmptyResult();

            if (!_asyncEditorService.IsAuthorizedToEdit(part, group)) return UnauthorizedGroupEditorResult();

            if (publish && !_asyncEditorService.IsAuthorizedToPublish(part, group))
            {
                return ErrorResult(T("You are not authorized to publish this content item."));
            }

            if (!string.IsNullOrEmpty(group) &&
                !_asyncEditorService.IsEditorGroupAvailable(part, group)) return GroupUnavailableResult();

            if (part.Id == 0) _contentManager.Create(part.ContentItem, VersionOptions.Draft);
            _contentManager.UpdateEditor(part.ContentItem, this, group);

            if (!ModelState.IsValid)
            {
                _transactionManager.Cancel();

                return AsyncEditorSaveResult(part, group, part.Id != 0);
            }

            if (part.HasEditorGroups)
            {
                _asyncEditorService.StoreCompletedEditorGroup(part, group);
            }

            var currentEditorGroupDescriptor = _asyncEditorService.GetEditorGroupDescriptor(part, group);
            var isPublishGroup = currentEditorGroupDescriptor?.IsPublishGroup ?? true;
            if (publish && isPublishGroup)
            {
                _contentManager.Publish(part.ContentItem);
            }

            return AsyncEditorSaveResult(
                part,
                group,
                true,
                !isPublishGroup && publish ? T("The current group is not a publish group. Draft has been saved.") : null);
        }

        protected virtual ActionResult SaveDraftOrPublishResult(int contentItemId, string group = "", string contentType = "", bool publish = false) =>
            SaveDraftOrPublishResult(GetAsyncEditorPart(contentItemId, contentType), group, publish);

        protected virtual ActionResult SaveAndNextResult(AsyncEditorPart part, string group)
        {
            if (string.IsNullOrEmpty(group)) return GroupNameCannotBeEmptyResult();
            
            if (part == null || !part.HasEditorGroups) return ContentItemNotFoundResult();

            if (!_asyncEditorService.IsAuthorizedToEdit(part, group)) return UnauthorizedGroupEditorResult();

            if (!_asyncEditorService.IsEditorGroupAvailable(part, group)) return GroupUnavailableResult();

            if (part.Id == 0) _contentManager.Create(part.ContentItem, VersionOptions.Draft);
            _contentManager.UpdateEditor(part.ContentItem, this, group);

            if (!ModelState.IsValid)
            {
                _transactionManager.Cancel();

                return AsyncEditorSaveResult(part, group, part.Id != 0);
            }

            _asyncEditorService.StoreCompletedEditorGroup(part, group);

            var nextGroup = _asyncEditorService.GetNextGroupDescriptor(part, group);
            if (nextGroup == null) return AsyncEditorResult(part, group);

            return AsyncEditorSaveResult(part, nextGroup.Name);
        }

        protected virtual ActionResult SaveAndNextResult(int contentItemId, string group, string contentType = "") =>
            SaveAndNextResult(GetAsyncEditorPart(contentItemId, contentType), group);

        protected virtual AsyncEditorPart GetAsyncEditorPart(int id, string contentType) =>
            id == 0 ?
                _contentManager.New<AsyncEditorPart>(contentType) :
                _contentManager.Get<AsyncEditorPart>(id, VersionOptions.Latest);

        protected virtual string GetEditorShapeHtml(AsyncEditorPart part, string group, bool contentCreated = true)
        {
            var editorShape = _asyncEditorService.BuildAsyncEditorShape(part, group);
            var asyncEditorShape = _shapeFactory.AsyncEditor_Editor(
                ValidationSummaryShape: _shapeFactory.AsyncEditor_ValidationSummary(ModelState: ModelState),
                EditorShape: editorShape,
                ContentItem: part.ContentItem,
                ContentCreated: contentCreated,
                Group: group);

            return _shapeDisplay.Display(asyncEditorShape);
        }

        #endregion

        #region Success results

        protected virtual ActionResult AsyncEditorResult(
            AsyncEditorPart part,
            string group,
            bool contentCreated = true,
            LocalizedString message = null) =>
            Json(new AsyncEditorResult
            {
                Success = true,
                ContentItemId = contentCreated ? part.ContentItem.Id : 0,
                EditorShape = GetEditorShapeHtml(part, group, contentCreated),
                EditorGroup = group,
                ResultMessage = message?.Text
            }, JsonRequestBehavior.AllowGet);

        protected virtual ActionResult AsyncEditorSaveResult(
            AsyncEditorPart part,
            string group,
            bool contentCreated = true,
            LocalizedString message = null) =>
            Json(new AsyncEditorSaveResult
            {
                Success = true,
                ContentItemId = contentCreated ? part.ContentItem.Id : 0,
                EditorShape = GetEditorShapeHtml(part, group, contentCreated),
                EditorGroup = group,
                HasValidationErrors = !ModelState.IsValid,
                ResultMessage = message?.Text
            }, JsonRequestBehavior.AllowGet);

        #endregion

        #region Error results

        protected virtual ActionResult ErrorResult(LocalizedString errorMessage) =>
            Json(new AsyncEditorResult
            {
                Success = false,
                ResultMessage = errorMessage.Text,
            }, JsonRequestBehavior.AllowGet);

        protected virtual ActionResult ContentItemNotFoundResult() =>
            ErrorResult(T("Content item was not found."));

        protected virtual ActionResult GroupNameCannotBeEmptyResult() =>
            ErrorResult(T("Group name cannot be empty."));

        protected virtual ActionResult UnauthorizedGroupEditorResult() =>
            ErrorResult(T("You are not authorized to edit this content item on this editor group."));

        protected virtual ActionResult GroupUnavailableResult() =>
            ErrorResult(T("Editor group is not available. Fill all the previous editor groups first."));

        #endregion

        #region IUpdateModel

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) =>
            TryUpdateModel(model, prefix, includeProperties, excludeProperties);

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) =>
            ModelState.AddModelError(key, errorMessage.ToString());

        #endregion IUpdateModel
    }
}