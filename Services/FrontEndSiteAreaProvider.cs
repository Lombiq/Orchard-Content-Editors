using Orchard.DisplayManagement.Descriptors;

namespace Lombiq.EditorGroups.Services
{
    public class FrontEndSiteAreaProvider : ISiteAreaProvider
    {
        public bool CanMatch(string area) => area == "FrontEnd";

        public bool SiteAreaMatched(ShapePlacementContext context) => !context.Path.StartsWith("~/Admin");
    }
}