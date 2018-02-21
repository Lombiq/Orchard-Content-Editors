using Orchard;
using Orchard.DisplayManagement.Descriptors;

namespace Lombiq.EditorGroups.Services
{
    public interface IRenderContextMatchProvider : IDependency
    {
        bool CanMatch(string renderContext);
        bool MatchRenderContext(ShapePlacementContext context);
    }
}