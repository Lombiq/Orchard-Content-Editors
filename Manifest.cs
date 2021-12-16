using Lombiq.ContentEditors.Constants;
using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "Lombiq Content Editors",
    Author = "Lombiq Technologies",
    Version = "1.0",
    Website = "https://github.com/Lombiq/Orchard-Content-Editors"
)]

[assembly: Feature(
    Id = FeatureIds.AsyncEditor,
    Name = "Lombiq Async Editor",
    Description = "Managing advanced content editing processes based on editor groups and asynchronous operations.",
    Category = "Content",
    Dependencies = new[]
    {
        "OrchardCore.ContentFields",
    }
)]

[assembly: Feature(
    Id = FeatureIds.Demo,
    Name = "Lombiq Content Editors Demo",
    Description = "Demonstration for Lombiq Content Editors features (e.g., Lombiq Async Editor). Suitable for UI testing as well.",
    Category = "Content",
    Dependencies = new[]
    {
        FeatureIds.AsyncEditor,
    }
)]
