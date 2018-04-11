using Orchard.ContentManagement;
using Orchard.Utility.Extensions;
using System.Web.Mvc.Html;

namespace System.Web.Mvc
{
    public static class HtmlHelperExtensions
    {
        public static string AsyncEditorPluginId(this HtmlHelper html, ContentItem contentItem) =>
            AsyncEditorPluginId(html, contentItem.ContentType);

        public static string AsyncEditorPluginId(this HtmlHelper html, string contentType) =>
            "asyncEditorPluginFor" + contentType;

        public static string ClassifiedName(this HtmlHelper html, string name) =>
            ClassifiedName(html, name, html.NameForModel().ToHtmlString());

        public static string ClassifiedName(this HtmlHelper html, string name, string nameForModel) =>
            $"{nameForModel}-{name}".HtmlClassify();
    }
}