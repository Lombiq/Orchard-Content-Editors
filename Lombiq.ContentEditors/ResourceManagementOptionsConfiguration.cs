using Lombiq.ContentEditors.Constants;
using Microsoft.Extensions.Options;
using OrchardCore.ResourceManagement;
using static Lombiq.ContentEditors.Constants.FeatureIds;

namespace Lombiq.ContentEditors;

public class ResourceManagementOptionsConfiguration : IConfigureOptions<ResourceManagementOptions>
{
    private static readonly ResourceManifest _manifest = new();

    static ResourceManagementOptionsConfiguration() =>
        _manifest
            .DefineScriptModule(ResourceNames.AsyncEditor)
            .SetUrl($"~/{Area}/js/async-editor.mjs")
            .SetVersion("2.0.0");

    public void Configure(ResourceManagementOptions options) => options.ResourceManifests.Add(_manifest);
}
