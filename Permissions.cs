using OrchardCore.Security.Permissions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Etch.OrchardCore.Greenhouse
{
    public class Permissions : IPermissionProvider
    {
        public static readonly Permission ManageGreenhouseSettings = new Permission("ManageGreenhouseSettings", "Manage Greenhouse Settings");

        public Task<IEnumerable<Permission>> GetPermissionsAsync()
        {
            return Task.FromResult(new[] { ManageGreenhouseSettings }.AsEnumerable());
        }

        public IEnumerable<PermissionStereotype> GetDefaultStereotypes()
        {
            return new[]
            {
                new PermissionStereotype
                {
                    Name = "Administrator",
                    Permissions = new[] { ManageGreenhouseSettings }
                }
            };
        }

    }
}
