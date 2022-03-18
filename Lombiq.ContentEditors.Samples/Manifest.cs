using Lombiq.ContentEditors.Constants;
using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "Lombiq Content Editors - Samples",
    Author = "Lombiq Technologies",
    Version = "1.0",
    Website = "https://github.com/Lombiq/Orchard-Content-Editors",
    Description = "Samples for Lombiq Content Editors.",
    Category = "Development",
    Dependencies = new[] { FeatureIds.AsyncEditor }
)]
