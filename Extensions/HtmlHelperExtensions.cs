using Orchard.ContentManagement;

namespace System.Web.Mvc
{
    public static class HtmlHelperExtensions
    {
        public static string AsyncEditorPluginId(this HtmlHelper html, ContentItem contentItem) =>
            AsyncEditorPluginId(html, contentItem.ContentType);

        public static string AsyncEditorPluginId(this HtmlHelper html, string contentType) =>
            "asyncEditorPluginFor" + contentType;
    }
}