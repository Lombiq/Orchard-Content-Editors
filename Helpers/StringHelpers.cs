using System.Linq;

namespace Lombiq.ContentEditors.Helpers
{
    public static class StringHelpers
    {
        public static string BuildMultiLineString(string lineSeparator, params string[] lines) =>
            string.Join(lineSeparator, lines.Where(line => !string.IsNullOrEmpty(line)));
    }
}