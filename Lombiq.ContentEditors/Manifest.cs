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
