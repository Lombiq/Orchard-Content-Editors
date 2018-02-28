using Orchard.DisplayManagement.Descriptors;
using Orchard.Mvc;
using Orchard.UI.Admin;

namespace Lombiq.EditorGroups.Services
{
    public class AdminRenderContextMatchProvider : IRenderContextMatchProvider
    {
        private readonly IHttpContextAccessor _hca;


        public AdminRenderContextMatchProvider(IHttpContextAccessor hca)
        {
            _hca = hca;
        }


        public bool CanMatch(string renderContext) => renderContext == "Admin";

        public bool MatchesRenderContext(ShapePlacementContext context) => 
            AdminFilter.IsApplied(_hca.Current().Request.RequestContext);
    }
}