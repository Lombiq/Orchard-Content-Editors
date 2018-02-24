using Orchard.UI.Resources;
using static Lombiq.EditorGroups.Constants.ResourceNames;

namespace Lombiq.EditorGroups
{
    public class ResourceManifest : IResourceManifestProvider
    {
        public void BuildManifests(ResourceManifestBuilder builder)
        {
            var manifest = builder.Add();
            
            manifest.DefineScript(Lombiq_AsyncEditor).SetUrl("lombiq-asynceditor.js").SetDependencies("jQuery");
        }
    }
}