; (function ($, window, document, undefined) {
    $.fn.extend({
        dynamicComparer: function (value, matchValue, noMatchValue) {
            let match;

            if (typeof value === "undefined" || value === null || value === "") {
                match = null;
            }
            else if (typeof value === "boolean") {
                match = value;
            }
            else if (typeof value === typeof matchValue) {
                match = value === matchValue;
            }
            else if (typeof value === typeof noMatchValue) {
                match = value !== noMatchValue;
            }
            else if (typeof value === "string" && typeof matchValue === "undefined" && typeof noMatchValue === "undefined") {
                // When there are no values supplied to compare the current value with, try to interpret the value as a boolean.
                match = value === "true" || value === "True" || value === "false" || value === "False" ?
                    value === "true" || value === "True" :
                    null;
            }
            else if (Array.isArray(matchValue)) {
                match = matchValue.includes(value);
            }
            else if (Array.isArray(noMatchValue)) {
                match = !noMatchValue.includes(value);
            }
            else if (Array.isArray(matchValue) && Array.isArray(value)) {
                match = matchValue.some(item => value.includes(item));
            }
            else if (Array.isArray(noMatchValue) && Array.isArray(value)) {
                match = !noMatchValue.some(item => value.includes(item));
            }

            return match;
        }
    });
})(jQuery, window, document);