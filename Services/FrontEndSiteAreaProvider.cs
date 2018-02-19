using Orchard.DisplayManagement.Descriptors;
using Orchard.Mvc;
using Orchard.UI.Admin;

namespace Lombiq.EditorGroups.Services
{
    public class FrontEndSiteAreaProvider : ISiteAreaProvider
    {
        private readonly IHttpContextAccessor _hca;


        public FrontEndSiteAreaProvider(IHttpContextAccessor hca)
        {
            _hca = hca;
        }


        public bool CanMatch(string area) => area == "FrontEnd";

        public bool SiteAreaMatched(ShapePlacementContext context) => !AdminFilter.IsApplied(_hca.Current().Request.RequestContext);
    }
}