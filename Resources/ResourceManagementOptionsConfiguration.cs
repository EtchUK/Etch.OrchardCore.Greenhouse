using Microsoft.Extensions.Options;
using OrchardCore.ResourceManagement;

namespace Etch.OrchardCore.Greenhouse.Resources
{
    public class ResourceManagementOptionsConfiguration : IConfigureOptions<ResourceManagementOptions>
    {
        private static ResourceManifest _manifest = new();

        static ResourceManagementOptionsConfiguration()
        {
            _manifest
                .DefineScript("greenhouse")
                .SetUrl("~/Etch.OrchardCore.Greenhouse/js/greenhouse.js");
        }

        public void Configure(ResourceManagementOptions options)
        {
            options.ResourceManifests.Add(_manifest);
        }
    }
}
