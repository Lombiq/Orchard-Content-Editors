using Orchard.DisplayManagement.Descriptors;
using Orchard.Mvc;
using Orchard.UI.Admin;

namespace Lombiq.ContentEditors.Services
{
    public class AdminRenderContextMatchProvider : IRenderContextMatchProvider
    {
        public const string RenderContext = "Admin";


        private readonly IHttpContextAccessor _hca;


        public AdminRenderContextMatchProvider(IHttpContextAccessor hca)
        {
            _hca = hca;
        }


        public bool CanMatch(string renderContext) => renderContext == RenderContext;

        public bool MatchesRenderContext(ShapePlacementContext context) => 
            AdminFilter.IsApplied(_hca.Current().Request.RequestContext);
    }
}