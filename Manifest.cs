using Lombiq.ContentEditors.Constants;
using OrchardCore.Modules.Manifest;

[assembly: Module(
    Name = "Lombiq Content Editors",
    Author = "Lombiq Technologies",
    Version = "1.0",
    Website = "https://github.com/Lombiq/Orchard-Content-Editors"
)]

[assembly: Feature(
    Id = FeatureIds.ContentEditors,
    Name = "Lombiq Content Editors",
    Description = "Managing advanced content editing processes based on editor groups.",
    Category = "Content",
    Dependencies = new[]
    {
        "OrchardCore.ContentFields",
    }
)]
