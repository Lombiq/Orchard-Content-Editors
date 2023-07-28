﻿using System.Collections.Generic;
using System.Linq;

namespace Lombiq.ContentEditors.Helpers
{
    public static class HtmlAttributeHelpers
    {
        public static void AddHtmlAttribute(ref IDictionary<string, object> htmlAttributes, string attribute, params string[] values)
        {
            if (string.IsNullOrEmpty(attribute) || !values.Any()) return;

            var valuesString = string.Join(" ", values);

            if (htmlAttributes == null) htmlAttributes = new Dictionary<string, object>();

            if (htmlAttributes.ContainsKey(attribute)) htmlAttributes[attribute] += valuesString;
            else htmlAttributes.Add(attribute, valuesString);
        }

        public static string StringifyHtmlAttributes(IDictionary<string, object> htmlAttributes)
        {
            if (htmlAttributes == null) return "";

            var stringHtmlAttributes = "";

            foreach (var attribute in htmlAttributes) stringHtmlAttributes += $"{attribute.Key}=\"{attribute.Value}\" ";

            return stringHtmlAttributes;
        }
    }
}