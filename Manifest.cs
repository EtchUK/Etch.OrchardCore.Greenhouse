using OrchardCore.Modules.Manifest;

[assembly: Module(
    Author = "Etch UK",
    Category = "Content",
    Description = "Integrates Greenhouse with Orchard Core.",
    Name = "Greenhouse",
    Version = "$(VersionNumber)",
    Website = "https://etchuk.com"
)]

[assembly: Feature(
    Id = "Etch.OrchardCore.Greenhouse",
    Name = "Greenhouse",
    Description = "Integrates Greenhouse with Orchard Core.",
    Category = "Content",
    Dependencies = new string[] {
        "Etch.OrchardCore.Fields.Query",
        "OrchardCore.Search.Lucene",
        "OrchardCore.ReCaptcha",
        "OrchardCore.Workflows"
    }
)]
