using Orchard.UI.Resources;
using static Lombiq.ContentEditors.Constants.ResourceNames;

namespace Lombiq.ContentEditors
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();

            manifest.DefineScript(Uri_Js).SetUrl("../Content/URI.js/URI.min.js", "../Content/URI.js/URI.js").SetDependencies("jQuery");

            manifest.DefineScript(iOS_StickyInputFocusOverride).SetUrl("lombiq-ios-stickyinputfocusoverride.js").SetDependencies(jQuery);

            manifest.DefineScript(Selectize).SetUrl("../Content/Selectize/selectize.min.js", "../Content/Selectize/selectize.js").SetDependencies(jQuery, iOS_StickyInputFocusOverride);
            manifest.DefineStyle(Selectize).SetUrl("../Content/Selectize/selectize.bootstrap3.css");

            manifest.DefineScript(AreYouSure).SetUrl("../Content/AreYouSure/jquery.are-you-sure.js").SetDependencies(jQuery);

            manifest.DefineScript(Lombiq_jQuery_EnableDisable).SetUrl("lombiq-jquery-enabledisable.js").SetDependencies(jQuery);

            manifest.DefineScript(Lombiq_LoadingIndicator).SetUrl("lombiq-loadingindicator.js").SetDependencies(jQuery);
            manifest.DefineStyle(Lombiq_LoadingIndicator).SetUrl("lombiq-loadingindicator.min.css", "lombiq-loadingindicator.css");

            manifest.DefineScript(Lombiq_AsyncEditor).SetUrl("lombiq-asynceditor.js").SetDependencies(Lombiq_LoadingIndicator, Uri_Js, AreYouSure);
            manifest.DefineScript(Lombiq_AsyncEditorWrapper).SetUrl("lombiq-asynceditorwrapper.js").SetDependencies(Lombiq_LoadingIndicator, Uri_Js);
            manifest.DefineScript(Lombiq_CheckboxListEditor).SetUrl("lombiq-checkboxlisteditor.js").SetDependencies(Lombiq_jQuery_EnableDisable);
            manifest.DefineScript(Lombiq_Editors_DateTimeEditor).SetUrl("lombiq-editors-datetimeeditor.js").SetDependencies(jQueryUI, Moment);
            manifest.DefineScript(Lombiq_BoolEditor).SetUrl("lombiq-booleditor.js").SetDependencies(jQuery);
            manifest.DefineScript(Lombiq_UnsafeAction).SetUrl("lombiq-unsafeaction.js").SetDependencies(jQuery);

            manifest.DefineScript(Lombiq_BoolEditor_Toggle).SetUrl("../Content/LC-switch/lc_switch.min.js", "../Content/LC-switch/lc_switch.js").SetDependencies(jQuery);
            manifest.DefineStyle(Lombiq_BoolEditor_Toggle).SetUrl("../Content/LC-switch/lc_switch.css");

            manifest.DefineScript(Lombiq_ReplaceElementAttribute).SetUrl("lombiq-replaceelementattribute.js").SetDependencies(jQuery);
            manifest.DefineScript(Lombiq_ConnectedValueSelector).SetUrl("lombiq-connectedvalueselector.js").SetDependencies(jQuery);
            manifest.DefineScript(Lombiq_ConnectedElementVisibility).SetUrl("lombiq-connectedelementvisibility.js").SetDependencies(Lombiq_ReplaceElementAttribute);
            manifest.DefineScript(Lombiq_ConnectedElementEnabledState).SetUrl("lombiq-connectedelementenabledstate.js").SetDependencies(jQuery);
            manifest.DefineScript(Lombiq_DisableEmptyFormInputs).SetUrl("lombiq-disableemptyforminputs.js").SetDependencies(jQuery);

            manifest.DefineStyle(Lombiq_Editors_DateTimeEditor).SetDependencies(jQueryUI);
        }
    }
}