using Lombiq.ContentEditors.Constants;
using Microsoft.Extensions.Options;
using OrchardCore.ResourceManagement;
using static Lombiq.ContentEditors.Constants.FeatureIds;
using static Lombiq.ContentEditors.Constants.ResourceNames;

namespace Lombiq.ContentEditors;

public class ResourceManagementOptionsConfiguration : IConfigureOptions<ResourceManagementOptions>
{
    private static readonly ResourceManifest _manifest = new();

    static ResourceManagementOptionsConfiguration()
    {
        _manifest
            .DefineScript(VueRouter)
            .SetDependencies(VueJs)
            .SetUrl(
                $"~/{Area}/vendors/vue-router/vue-router.min.js",
                $"~/{Area}/vendors/vue-router/vue-router.js")
            .SetVersion("3.5.3");

        _manifest
            .DefineScript(ResourceNames.AsyncEditor)
            .SetDependencies(VueRouter)
            .SetUrl($"~/{Area}/lombiq/async-editor.min.js", $"~/{Area}/lombiq/async-editor.js")
            .SetVersion("1.0.0");
    }

    public void Configure(ResourceManagementOptions options) => options.ResourceManifests.Add(_manifest);
}
