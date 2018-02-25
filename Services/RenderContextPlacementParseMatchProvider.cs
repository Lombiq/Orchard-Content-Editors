using Lombiq.EditorGroups.Services;
using Orchard.DisplayManagement.Descriptors;
using Orchard.DisplayManagement.Descriptors.ShapePlacementStrategy;
using System.Collections.Generic;
using System.Linq;

namespace Lombiq.EditorGroups.Services
{
    public class RenderContextPlacementParseMatchProvider : IPlacementParseMatchProvider
    {
        private readonly IEnumerable<IRenderContextMatchProvider> _siteAreaProviders;


        public RenderContextPlacementParseMatchProvider(IEnumerable<IRenderContextMatchProvider> siteAreaProviders)
        {
            _siteAreaProviders = siteAreaProviders;
        }


        public string Key => "RenderContext";

        public bool Match(ShapePlacementContext context, string expression) =>
            _siteAreaProviders
                .FirstOrDefault(siteAreaProvider => 
                    siteAreaProvider.CanMatch(expression))?.MatchRenderContext(context) ?? false;
    }
}