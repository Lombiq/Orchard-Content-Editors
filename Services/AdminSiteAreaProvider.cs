using Orchard.DisplayManagement.Descriptors;
using Orchard.Mvc;
using Orchard.UI.Admin;

namespace Lombiq.EditorGroups.Services
{
    public class AdminSiteAreaProvider : ISiteAreaProvider
    {
        private readonly IHttpContextAccessor _hca;


        public AdminSiteAreaProvider(IHttpContextAccessor hca)
        {
            _hca = hca;
        }


        public bool CanMatch(string area) => area == "Admin";

        public bool SiteAreaMatched(ShapePlacementContext context) => AdminFilter.IsApplied(_hca.Current().Request.RequestContext);
    }
}