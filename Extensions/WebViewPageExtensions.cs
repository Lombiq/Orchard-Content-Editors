using Orchard.Mvc.ViewEngines.Razor;

namespace Lombiq.ContentEditors.Extensions
{
    public static class WebViewPageExtensions
    {
        // https://stackoverflow.com/questions/37206185/server-side-detection-if-the-browser-is-internet-explorer-in-asp-net-core/37210073
        public static bool IsInternetExplorer(this WebViewPage<dynamic> page, string userAgent) =>
            userAgent.Contains("MSIE") || userAgent.Contains("Trident");
    }
}