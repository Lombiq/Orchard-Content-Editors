using Orchard.DisplayManagement.Descriptors;
using Orchard.Mvc;
using Orchard.UI.Admin;

namespace Lombiq.ContentEditors.Services
{
    public class FrontEndRenderContextMatchProvider : IRenderContextMatchProvider
    {
        public const string RenderContext = "FrontEnd";


        private readonly IHttpContextAccessor _hca;


        public FrontEndRenderContextMatchProvider(IHttpContextAccessor hca)
        {
            _hca = hca;
        }


        public bool CanMatch(string renderContext) => renderContext == RenderContext;

        public bool MatchesRenderContext(ShapePlacementContext context) => 
            !AdminFilter.IsApplied(_hca.Current().Request.RequestContext);
    }
}