using Orchard.Localization;
using System.Web;
using static Lombiq.ContentEditors.Constants.ElementNames;

namespace Lombiq.ContentEditors.ViewModels
{
    public class AsyncEditorLoaderViewModel
    {
        public string AsyncEditorLoaderId { get; set; }

        public int? ContentItemId { get; set; }

        public string ContentType { get; set; }

        public string EditorGroup { get; set; }

        public string EditUrl { get; set; }

        public bool UseStaticLoadingIndicator { get; set; }

        public string AsyncEditorLoaderElementClass { get; } = ".js-" + AsyncEditorLoader.BlockName;

        public string ProcessingIndicatorElementClass { get; set; }

        public string EditorPlaceholderElementClass { get; } = ".js-" + AsyncEditorLoader.EditorPlaceholderElementName;

        public string LoadEditorActionElementClass { get; } = ".js-" + AsyncEditorActions.LoadEditorElementName;

        public string PostEditorActionElementClass { get; } = ".js-" + AsyncEditorActions.PostEditorElementName;

        public string DirtyFormLeaveConfirmationText { get; } = HttpUtility.JavaScriptStringEncode(
            new LocalizedString("Are you sure you want to leave this editor group? Changes you made may not be saved.").Text);
    }
}