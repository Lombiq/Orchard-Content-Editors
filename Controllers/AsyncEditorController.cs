using Lombiq.ContentEditors.Services;
using Orchard.ContentManagement;
using Orchard.Data;
using Orchard.DisplayManagement;
using Orchard.Mvc;
using System.Web.Mvc;

namespace Lombiq.ContentEditors.Controllers
{
    public class AsyncEditorController : AsyncEditorControllerBase
    {
        public AsyncEditorController(
            IContentManager contentManager, 
            IShapeDisplay shapeDisplay,
            IShapeFactory shapeFactory,
            IAsyncEditorService asyncEditorService,
            ITransactionManager transactionManager)
            : base(contentManager, shapeDisplay, shapeFactory, asyncEditorService, transactionManager)
        {
        }


        public ActionResult Edit(int contentItemId, string group = "", string contentType = "") =>
            EditResult(contentItemId, group, contentType);

        [HttpPost, ActionName(nameof(AsyncEditorController.Edit))]
        [FormValueRequired("submit.Save")]
        public ActionResult SavePost(int contentItemId, string group = "", string contentType = "") =>
            SaveDraftOrPublishResult(contentItemId, group, contentType, false);

        [HttpPost, ActionName(nameof(AsyncEditorController.Edit))]
        [FormValueRequired("submit.Publish")]
        public ActionResult PublishPost(int contentItemId, string group = "", string contentType = "") =>
            SaveDraftOrPublishResult(contentItemId, group, contentType, true);

        [HttpPost, ActionName(nameof(AsyncEditorController.Edit))]
        [FormValueRequired("submit.SaveAndNext")]
        public ActionResult SaveAndNextPost(int contentItemId, string group, string contentType = "") =>
            SaveAndNextResult(contentItemId, group, contentType);
    }
}