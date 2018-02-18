using Orchard;
using Orchard.DisplayManagement.Descriptors;

namespace Lombiq.EditorGroups.Services
{
    public interface ISiteAreaProvider : IDependency
    {
        bool CanMatch(string area);
        bool SiteAreaMatched(ShapePlacementContext context);
    }
}