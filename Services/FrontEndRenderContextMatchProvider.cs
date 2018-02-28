using Orchard.DisplayManagement.Descriptors;
using Orchard.Mvc;
using Orchard.UI.Admin;

namespace Lombiq.EditorGroups.Services
{
    public class FrontEndRenderContextMatchProvider : IRenderContextMatchProvider
    {
        private readonly IHttpContextAccessor _hca;


        public FrontEndRenderContextMatchProvider(IHttpContextAccessor hca)
        {
            _hca = hca;
        }


        public bool CanMatch(string renderContext) => renderContext == "FrontEnd";

        public bool MatchRenderContext(ShapePlacementContext context) => 
            !AdminFilter.IsApplied(_hca.Current().Request.RequestContext);
    }
}