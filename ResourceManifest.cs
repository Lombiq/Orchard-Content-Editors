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

            manifest.DefineScript(Lombiq_AsyncEditor).SetUrl("lombiq-asynceditor.js").SetDependencies(jQuery, Uri_Js);
            manifest.DefineScript(Lombiq_AsyncEditorWrapper).SetUrl("lombiq-asynceditorwrapper.js").SetDependencies(jQuery, Uri_Js);
        }
    }
}