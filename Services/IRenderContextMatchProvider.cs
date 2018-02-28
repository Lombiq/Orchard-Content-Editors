using Orchard;
using Orchard.DisplayManagement.Descriptors;

namespace Lombiq.EditorGroups.Services
{
    public interface IRenderContextMatchProvider : IDependency
    {
        /// <summary>
        /// Checks if the current provider can handle the given render context.
        /// </summary>
        /// <param name="renderContext">Render context that needs to be checked.</param>
        /// <returns>True if this provider can handle the given render context.</returns>
        bool CanMatch(string renderContext);

        /// <summary>
        /// Evaluates the given placement context and checks if the current context matches for the render context.
        /// </summary>
        /// <param name="context">Placement context that needs to be evaluated.</param>
        /// <returns>True if the context matches.</returns>
        bool MatchesRenderContext(ShapePlacementContext context);
    }
}