using static Lombiq.ContentEditors.Constants.AsyncEditorConstants;

namespace Lombiq.ContentEditors.Extensions
{
    public static class StringExtensions
    {
        public static string AsyncEditorPluginId(this string contentType) => AsyndEditorPluginIdPrefix + contentType;
    }
}