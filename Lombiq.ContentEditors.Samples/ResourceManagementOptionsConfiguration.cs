using Microsoft.Extensions.Options;
using OrchardCore.ResourceManagement;
using static Lombiq.ContentEditors.Samples.Constants.FeatureIds;
using static Lombiq.ContentEditors.Samples.Constants.ResourceNames;

namespace Lombiq.ContentEditors.Samples;

public class ResourceManagementOptionsConfiguration : IConfigureOptions<ResourceManagementOptions>
{
    private static readonly ResourceManifest _manifest = new();

    static ResourceManagementOptionsConfiguration() =>
        _manifest
            .DefineScript(SampleAsyncEditorGroupScript)
            .SetUrl($"~/{Area}/js/sample.js")
            .SetVersion("2.0.0");

    public void Configure(ResourceManagementOptions options) => options.ResourceManifests.Add(_manifest);
}
