using OrchardCore.Modules.Manifest;

[assembly: Module(
    Author = "Etch UK",
    Category = "Content",
    Description = "Integrates Greenhouse with Orchard Core.",
    Name = "Greenhouse",
    Version = "0.0.1",
    Website = "https://etchuk.com"
)]

[assembly: Feature(
    Id = "Etch.OrchardCore.Greenhouse",
    Name = "Greenhouse",
    Description = "Integrates Greenhouse with Orchard Core.",
    Category = "Content",
    Dependencies = new string[] { "OrchardCore.Workflows" }
)]