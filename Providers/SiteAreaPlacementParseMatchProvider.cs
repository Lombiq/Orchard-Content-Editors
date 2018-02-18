using Lombiq.EditorGroups.Services;
using Orchard.DisplayManagement.Descriptors;
using Orchard.DisplayManagement.Descriptors.ShapePlacementStrategy;
using System.Collections.Generic;
using System.Linq;

namespace Lombiq.EditorGroups.Providers
{
    public class SiteAreaPlacementParseMatchProvider : IPlacementParseMatchProvider
    {
        private readonly IEnumerable<ISiteAreaProvider> _siteAreaProviders;


        public SiteAreaPlacementParseMatchProvider(IEnumerable<ISiteAreaProvider> siteAreaProviders)
        {
            _siteAreaProviders = siteAreaProviders;
        }


        public string Key => "SiteArea";

        public bool Match(ShapePlacementContext context, string expression) =>
            _siteAreaProviders
                .FirstOrDefault(siteAreaProvider => 
                    siteAreaProvider.CanMatch(expression))?.SiteAreaMatched(context) ?? false;
    }
}