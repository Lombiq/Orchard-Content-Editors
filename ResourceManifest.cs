using Orchard.UI.Resources;
using static Lombiq.ContentEditors.Constants.ResourceNames;

namespace Lombiq.ContentEditors
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();
            
            manifest.DefineScript(Lombiq_AsyncEditor).SetUrl("lombiq-asynceditor.js").SetDependencies("jQuery");
            manifest.DefineScript(Lombiq_AsyncEditorWrapper).SetUrl("lombiq-asynceditorwrapper.js").SetDependencies("jQuery");
        }
    }
}