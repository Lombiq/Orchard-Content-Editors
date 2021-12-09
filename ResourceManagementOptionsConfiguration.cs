using Microsoft.Extensions.Options;
using OrchardCore.ResourceManagement;
using static Lombiq.ContentEditors.Constants.FeatureIds;
using static Lombiq.ContentEditors.Constants.ResourceNames;

namespace Lombiq.ContentEditors
{
    public class ResourceManagementOptionsConfiguration : IConfigureOptions<ResourceManagementOptions>
    {
        private static readonly ResourceManifest _manifest = new();

        static ResourceManagementOptionsConfiguration()
        {
            _manifest
                .DefineScript(AsyncEditor)
                .SetDependencies(VueJs)
                .SetUrl($"~/{Area}/js/async-editor/async-editor.js")
                .SetVersion("1.0.0");
        }

        public void Configure(ResourceManagementOptions options) => options.ResourceManifests.Add(_manifest);
    }
}
