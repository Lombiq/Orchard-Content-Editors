﻿using Lombiq.ContentEditors.Models;
using Lombiq.ContentEditors.Services;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Mvc;
using System.Web.Mvc;

namespace Lombiq.ContentEditors.Controllers
{
    public class AsyncEditorController : Controller, IUpdateModel
    {
        private readonly IContentManager _contentManager;
        private readonly IShapeDisplay _shapeDisplay;
        private readonly dynamic _shapeFactory;
        private readonly IAsyncEditorService _asyncEditorService;
        private readonly ITransactionManager _transactionManager;


        public Localizer T { get; set; }


        public AsyncEditorController(
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


        public ActionResult Edit(int contentItemId, string group = "", string contentType = "")
        {
            var part = GetAsyncEditorPart(contentItemId, contentType);

            if (part == null) return ContentItemNotFoundResult();

            if (part.HasEditorGroups && string.IsNullOrEmpty(group)) return GroupNameCannotBeEmptyResult();

            if (!_asyncEditorService.IsAuthorizedToEdit(part, group)) return UnauthorizedGroupEditorResult();

            if (!string.IsNullOrEmpty(group) && 
                !_asyncEditorService.IsEditorGroupAvailable(part, group)) return GroupUnavailableResult();

            return AsyncEditorResult(part, group);
        }

        [HttpPost, ActionName(nameof(AsyncEditorController.Edit))]
        [FormValueRequired("submit.Save")]
        public ActionResult SavePost(int contentItemId, string group = "", string contentType = "") =>
            SaveDraftOrPublishPost(contentItemId, group, contentType, false);

        [HttpPost, ActionName(nameof(AsyncEditorController.Edit))]
        [FormValueRequired("submit.Publish")]
        public ActionResult PublishPost(int contentItemId, string group = "", string contentType = "") =>
            SaveDraftOrPublishPost(contentItemId, group, contentType, true);
        
        [HttpPost, ActionName(nameof(AsyncEditorController.Edit))]
        [FormValueRequired("submit.SaveAndNext")]
        public ActionResult SaveAndNextPost(int contentItemId, string group, string contentType = "")
        {
            if (string.IsNullOrEmpty(group)) return GroupNameCannotBeEmptyResult();

            var part = GetAsyncEditorPart(contentItemId, contentType);

            if (part == null || !part.HasEditorGroups) return ContentItemNotFoundResult();

            if (!_asyncEditorService.IsAuthorizedToEdit(part, group)) return UnauthorizedGroupEditorResult();

            if (!_asyncEditorService.IsEditorGroupAvailable(part, group)) return GroupUnavailableResult();

            if (contentItemId == 0) _contentManager.Create(part.ContentItem, VersionOptions.Draft);
            _contentManager.UpdateEditor(part.ContentItem, this, group);

            if (!ModelState.IsValid)
            {
                _transactionManager.Cancel();

                return AsyncEditorSaveResult(part, group, contentItemId != 0);
            }

            _asyncEditorService.StoreCompletedEditorGroup(part, group);

            var nextGroup = _asyncEditorService.GetNextGroupDescriptor(part, group);
            if (nextGroup == null) return AsyncEditorResult(part, group);

            return AsyncEditorSaveResult(part, nextGroup.Name);
        }

        #region Helpers

        private ActionResult SaveDraftOrPublishPost(int contentItemId, string group = "", string contentType = "", bool publish = false)
        {
            var part = GetAsyncEditorPart(contentItemId, contentType);

            if (part == null) return ContentItemNotFoundResult();

            if (part.HasEditorGroups && string.IsNullOrEmpty(group)) return GroupNameCannotBeEmptyResult();

            if (!_asyncEditorService.IsAuthorizedToEdit(part, group)) return UnauthorizedGroupEditorResult();

            if (publish && !_asyncEditorService.IsAuthorizedToPublish(part, group))
            {
                return ErrorResult(T("You are not authorized to publish this content item."));
            }

            if (!string.IsNullOrEmpty(group) &&
                !_asyncEditorService.IsEditorGroupAvailable(part, group)) return GroupUnavailableResult();

            if (contentItemId == 0) _contentManager.Create(part.ContentItem, VersionOptions.Draft);
            _contentManager.UpdateEditor(part.ContentItem, this, group);

            if (!ModelState.IsValid)
            {
                _transactionManager.Cancel();

                return AsyncEditorSaveResult(part, group, contentItemId != 0);
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

        private AsyncEditorPart GetAsyncEditorPart(int id, string contentType) =>
            id == 0 ? 
                _contentManager.New<AsyncEditorPart>(contentType) :
                _contentManager.Get<AsyncEditorPart>(id, VersionOptions.Latest);

        private string GetEditorShapeHtml(AsyncEditorPart part, string group, bool contentCreated = true)
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

        private ActionResult AsyncEditorResult(
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

        private ActionResult AsyncEditorSaveResult(
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

        private ActionResult ErrorResult(LocalizedString errorMessage) =>
            Json(new AsyncEditorResult
            {
                Success = false,
                ResultMessage = errorMessage.Text,
            }, JsonRequestBehavior.AllowGet);

        private ActionResult ContentItemNotFoundResult() =>
            ErrorResult(T("Content item was not found."));

        private ActionResult GroupNameCannotBeEmptyResult() =>
            ErrorResult(T("Group name cannot be empty."));

        private ActionResult UnauthorizedGroupEditorResult() =>
            ErrorResult(T("You are not authorized to edit this content item on this editor group."));

        private ActionResult GroupUnavailableResult() =>
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