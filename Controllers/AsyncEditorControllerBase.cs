using Lombiq.ContentEditors.Events;
using Lombiq.ContentEditors.Models;
using Lombiq.ContentEditors.Services;
using Orchard;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Localization;
using System;
using System.Web.Mvc;

namespace Lombiq.ContentEditors.Controllers
{
    public abstract class AsyncEditorControllerBase : Controller, IUpdateModel
    {
        protected readonly Lazy<WorkContext> _workContextLazy;
        protected readonly IAsyncEditorEventHandler _asyncEditorEventHandler;
        protected readonly IContentManager _contentManager;
        protected readonly IShapeDisplay _shapeDisplay;
        protected readonly dynamic _shapeFactory;
        protected readonly IAsyncEditorService _asyncEditorService;
        protected readonly ITransactionManager _transactionManager;


        public Localizer T { get; set; }


        public AsyncEditorControllerBase(
            IOrchardServices orchardServices,
            IShapeDisplay shapeDisplay,
            IAsyncEditorService asyncEditorService,
            IAsyncEditorEventHandler asyncEditorEventHandler)
        {
            _contentManager = orchardServices.ContentManager;
            _shapeDisplay = shapeDisplay;
            _shapeFactory = orchardServices.New;
            _asyncEditorService = asyncEditorService;
            _transactionManager = orchardServices.TransactionManager;
            _asyncEditorEventHandler = asyncEditorEventHandler;
            _workContextLazy = new Lazy<WorkContext>(() => orchardServices.WorkContext);

            T = NullLocalizer.Instance;
        }


        #region Helpers

        protected virtual ActionResult EditResult(AsyncEditorPart part, string group = "")
        {
            if (part == null) return ContentItemNotFoundResult();

            if (part.HasEditorGroups && string.IsNullOrEmpty(group))
            {
                group = part.GetLastDisplayedGroupDescriptor()?.Name ?? part.NextEditableAuthorizedGroup?.Name;

                if (string.IsNullOrEmpty(group)) return GroupNameCannotBeEmptyResult();
            }

            if (!_asyncEditorService.IsAuthorizedToEdit(part, group)) return UnauthorizedEditorResult();

            if (!string.IsNullOrEmpty(group) && !part.IsEditorGroupAvailable(group))
                return GroupUnavailableResult();

            return AsyncEditorResult(part, group);
        }

        protected virtual ActionResult EditResult(int contentItemId, string group = "", string contentType = "") =>
            EditResult(GetAsyncEditorPart(contentItemId, contentType), group);

        protected virtual ActionResult SaveDraftOrPublishResult(AsyncEditorPart part, string group = "", bool publish = false)
        {
            if (part == null) return ContentItemNotFoundResult();

            if (part.HasEditorGroups && string.IsNullOrEmpty(group)) return GroupNameCannotBeEmptyResult();

            if (!_asyncEditorService.IsAuthorizedToEdit(part, group)) return UnauthorizedEditorResult();

            if (publish && !_asyncEditorService.IsAuthorizedToPublish(part, group))
            {
                return ErrorResult(T("You are not authorized to publish this content item."));
            }

            if (!string.IsNullOrEmpty(group) && !part.IsEditorGroupAvailable(group))
                return GroupUnavailableResult();

            var newContent = part.Id == 0;
            if (newContent) _contentManager.Create(part.ContentItem, VersionOptions.Draft);

            _asyncEditorEventHandler.Updating(part, group, newContent);

            var editor = _contentManager.UpdateEditor(part.ContentItem, this, group);

            _asyncEditorEventHandler.Updated(part, group, newContent, ModelState.IsValid);

            if (!ModelState.IsValid)
            {
                _transactionManager.Cancel();

                return AsyncEditorSaveResult(part, group, !newContent, editor);
            }

            if (part.HasEditorGroups) part.StoreCompletedEditorGroup(group);

            part.LastUpdatedEditorGroupName = group;

            var currentEditorGroupDescriptor = part.GetEditorGroupDescriptor(group);
            var isPublishGroup = currentEditorGroupDescriptor?.IsPublishGroup ?? true;

            if (publish && isPublishGroup) _contentManager.Publish(part.ContentItem);

            _asyncEditorEventHandler.Saved(part, group, newContent, publish && isPublishGroup);

            return AsyncEditorSaveResult(
                part,
                group,
                true,
                null,
                !isPublishGroup && publish ? T("The current group is not a publish group. Draft has been saved.") : null);
        }

        protected virtual ActionResult SaveDraftOrPublishResult(int contentItemId, string group = "", string contentType = "", bool publish = false) =>
            SaveDraftOrPublishResult(GetAsyncEditorPart(contentItemId, contentType), group, publish);

        protected virtual ActionResult SaveAndNextResult(AsyncEditorPart part, string group)
        {
            if (string.IsNullOrEmpty(group)) return GroupNameCannotBeEmptyResult();

            if (part == null || !part.HasEditorGroups) return ContentItemNotFoundResult();

            if (!_asyncEditorService.IsAuthorizedToEdit(part, group)) return UnauthorizedEditorResult();

            if (!part.IsEditorGroupAvailable(group)) return GroupUnavailableResult();

            var newContent = part.Id == 0;
            if (newContent) _contentManager.Create(part.ContentItem, VersionOptions.Draft);

            _asyncEditorEventHandler.Updating(part, group, newContent);

            var editor = _contentManager.UpdateEditor(part.ContentItem, this, group);

            _asyncEditorEventHandler.Updated(part, group, newContent, ModelState.IsValid);

            if (!ModelState.IsValid)
            {
                _transactionManager.Cancel();

                return AsyncEditorSaveResult(part, group, !newContent, editor);
            }

            part.StoreCompletedEditorGroup(group);

            part.LastUpdatedEditorGroupName = group;

            _asyncEditorEventHandler.Saved(part, group, newContent, false);

            var nextGroup = part.GetNextGroupDescriptor(group);

            return nextGroup == null ? AsyncEditorResult(part, group) : AsyncEditorSaveResult(part, nextGroup.Name);
        }

        protected virtual ActionResult SaveAndNextResult(int contentItemId, string group, string contentType = "") =>
            SaveAndNextResult(GetAsyncEditorPart(contentItemId, contentType), group);

        protected virtual AsyncEditorPart GetAsyncEditorPart(int id, string contentType) =>
            id == 0 ?
                _contentManager.New<AsyncEditorPart>(contentType) :
                _contentManager.Get<AsyncEditorPart>(id, VersionOptions.Latest);

        protected virtual string GetEditorShapeHtml(AsyncEditorPart part, string group, bool contentCreated = true, dynamic shape = null)
        {
            part.SetCurrentEditorGroup(group);
            part.IsAsyncEditorContext = true;

            return _shapeDisplay.Display(_shapeFactory.AsyncEditor_Editor(
                ValidationSummaryShape: _shapeFactory.AsyncEditor_ValidationSummary(ModelState: ModelState),
                EditorShape: shape ?? _contentManager.BuildEditor(part, group),
                ContentItem: part.ContentItem,
                ContentCreated: contentCreated,
                Group: group));
        }

        protected virtual void SetEditorSessionCookieForAnonymousUser(AsyncEditorPart part)
        {
            if (_workContextLazy.Value.CurrentUser == null) _asyncEditorService.SetEditorSessionCookie(part);
        }

        protected virtual void RemoveEditorSessionCookieForAnonymousUser()
        {
            if (_workContextLazy.Value.CurrentUser == null) _asyncEditorService.RemoveEditorSessionCookie();
        }

        #endregion

        #region Success results

        protected virtual ActionResult AsyncEditorResult(
            AsyncEditorPart part,
            string group,
            LocalizedString message = null)
        {
            part.LastDisplayedEditorGroupName = group;

            if (part.Id == 0) RemoveEditorSessionCookieForAnonymousUser();
            else SetEditorSessionCookieForAnonymousUser(part);

            return Json(new AsyncEditorGroupResult
            {
                Success = true,
                ContentItemId = part.ContentItem.Id,
                EditorShape = GetEditorShapeHtml(part, group, part.ContentItem.Id != 0),
                EditorGroup = group,
                ResultMessage = message?.Text
            }, JsonRequestBehavior.AllowGet);
        }

        protected virtual ActionResult AsyncEditorSaveResult(
            AsyncEditorPart part,
            string group,
            bool contentCreated = true,
            dynamic shape = null,
            LocalizedString message = null)
        {
            part.LastDisplayedEditorGroupName = group;
            part.IsContentCreationFailed = part.Id != 0 && !contentCreated;

            if (contentCreated) SetEditorSessionCookieForAnonymousUser(part);
            else RemoveEditorSessionCookieForAnonymousUser();

            return Json(new AsyncEditorSaveResult
            {
                Success = true,
                ContentItemId = contentCreated ? part.ContentItem.Id : 0,
                EditorShape = GetEditorShapeHtml(part, group, contentCreated, shape),
                EditorGroup = group,
                HasValidationErrors = !ModelState.IsValid,
                ResultMessage = message?.Text
            }, JsonRequestBehavior.AllowGet);
        }

        protected virtual ActionResult SuccessResult(LocalizedString message = null) =>
            Json(new AsyncEditorResult
            {
                Success = true,
                ResultMessage = message?.Text
            });

        #endregion

        #region Error results

        protected virtual ActionResult ErrorResult(LocalizedString errorMessage) =>
            Json(new AsyncEditorGroupResult
            {
                Success = false,
                ResultMessage = errorMessage.Text,
            }, JsonRequestBehavior.AllowGet);

        protected virtual ActionResult ContentItemNotFoundResult() =>
            ErrorResult(T("Content item was not found."));

        protected virtual ActionResult GroupNameCannotBeEmptyResult() =>
            ErrorResult(T("Group name cannot be empty."));

        protected virtual ActionResult UnauthorizedEditorResult() =>
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