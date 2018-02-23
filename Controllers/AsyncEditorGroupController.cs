using Lombiq.EditorGroups.Constants;
using Lombiq.EditorGroups.Models;
using Lombiq.EditorGroups.Services;
using Orchard.ContentManagement;
using Orchard.Core.Contents;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Localization;
using Orchard.Security;
using Orchard.Themes;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Lombiq.EditorGroups.Controllers
{
    public class AsyncEditorGroupController : Controller, IUpdateModel
    {
        private readonly IContentManager _contentManager;
        private readonly IAuthorizer _authorizer;
        private readonly IShapeDisplay _shapeDisplay;
        private readonly dynamic _shapeFactory;
        private readonly IAsyncEditorService _asyncEditorService;
        private readonly ITransactionManager _transactionManager;


        public Localizer T { get; set; }


        public AsyncEditorGroupController(IContentManager contentManager, IAuthorizer authorizer, IShapeDisplay shapeDisplay, IShapeFactory shapeFactory, IAsyncEditorService asyncEditorService, ITransactionManager transactionManager)
        {
            _contentManager = contentManager;
            _authorizer = authorizer;
            _shapeDisplay = shapeDisplay;
            _shapeFactory = shapeFactory;
            _asyncEditorService = asyncEditorService;
            _transactionManager = transactionManager;

            T = NullLocalizer.Instance;
        }


        public ActionResult EditGroup(int contentItemId, string group, string contentType = "")
        {
            if (string.IsNullOrEmpty(group)) return GroupNameCannotBeEmptyResult();

            var part = GetEditorGroupsPart(contentItemId, contentType);

            if (!_asyncEditorService.IsAuthorizedToEditGroup(part, group)) return UnauthorizedGroupEditorResult();

            if (!_asyncEditorService.EditorGroupAvailable(part, group)) return GroupUnavailableResult();

            return EditorGroupResult(part, group);
        }

        [HttpPost, ActionName("EditGroup"), ValidateInput(false)]
        public ActionResult EditGroupPost(int contentItemId, string group, string contentType = "")
        {
            if (string.IsNullOrEmpty(group)) return GroupNameCannotBeEmptyResult();

            var part = GetEditorGroupsPart(contentItemId, contentType);

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
            
            if (_asyncEditorService.GetEditorGroupDescriptor(part, group).PublishGroup)
            {
                _contentManager.Publish(part.ContentItem);
            }

            var nextGroup = _asyncEditorService.GetNextEditorGroupDescriptor(part, group);
            if (nextGroup == null) return EditorGroupResult(part, group);

            return EditorGroupResult(part, nextGroup.Name);
        }


        private EditorGroupsPart GetEditorGroupsPart(int id, string contentType)
        {
            var item = _contentManager.Get(id, VersionOptions.Latest);

            item = item ?? _contentManager.New(contentType);

            return item.As<EditorGroupsPart>();
        }

        private string GetEditorShapeHtml(EditorGroupsPart part, string group)
        {
            var editorShape = _asyncEditorService.BuildAsyncEditorShape(part, group);
            var formShape = _shapeFactory.AsyncEditor_Editor(
                ValidationSummaryShape: _shapeFactory.AsyncEditor_ValidationSummary(ModelState: ModelState),
                EditorShape: editorShape, 
                ContentItem: part.ContentItem,
                Group: group);

            return _shapeDisplay.Display(formShape);
        }

        private ActionResult GroupNameCannotBeEmptyResult() =>
            ErrorResult(T("Group name cannot be empty."));

        private ActionResult UnauthorizedGroupEditorResult() =>
            ErrorResult(T("You don't have permission to edit this content item on this editor group."));

        private ActionResult GroupUnavailableResult() =>
            ErrorResult(T("Editor group is not available. Fill all the previous editor groups first."));

        private ActionResult ErrorResult(LocalizedString errorMessage) =>
            Json(new EditorGroupsResult
            {
                Success = false,
                ErrorMessage = errorMessage.Text,
            }, JsonRequestBehavior.AllowGet);

        private ActionResult EditorGroupResult(EditorGroupsPart part, string group) =>
            Json(new EditorGroupsResult
            {
                Success = true,
                ContentItemId = part.ContentItem.Id,
                EditorShape = GetEditorShapeHtml(part, group),
                EditorGroup = group
            }, JsonRequestBehavior.AllowGet);


        #region IUpdateModel

        bool IUpdateModel.TryUpdateModel<TModel>(TModel model, string prefix, string[] includeProperties, string[] excludeProperties) =>
            TryUpdateModel(model, prefix, includeProperties, excludeProperties);

        void IUpdateModel.AddModelError(string key, LocalizedString errorMessage) =>
            ModelState.AddModelError(key, errorMessage.ToString());

        #endregion IUpdateModel
    }
}