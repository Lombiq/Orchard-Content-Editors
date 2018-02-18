using Orchard.DisplayManagement.Descriptors;

namespace Lombiq.EditorGroups.Services
{
    public class AdminSiteAreaProvider : ISiteAreaProvider
    {
        public bool CanMatch(string area) => area == "Admin";

        public bool SiteAreaMatched(ShapePlacementContext context) => context.Path.StartsWith("~/Admin");
    }
}