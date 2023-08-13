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
            .SetDependencies("jQuery")
            .SetUrl($"~/{Area}/js/sample/sample.js")
            .SetVersion("1.0.0");

    public void Configure(ResourceManagementOptions options) => options.ResourceManifests.Add(_manifest);
}
