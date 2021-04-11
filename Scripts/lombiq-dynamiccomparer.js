; (function ($, window, document, undefined) {
    $.fn.extend({
        dynamicComparer: function (value, matchValue, noMatchValue) {
            let match;

            if (typeof value === "undefined") {
                match = null;
            }
            else if (typeof value === "boolean") {
                match = value;
            }
            else if (matchValue !== null && value === matchValue) {
                match = true;
            }
            else if (noMatchValue !== null && value === noMatchValue) {
                match = false;
            }
            else if (Array.isArray(matchValue) && matchValue.includes(value)) {
                match = true;
            }
            else if (Array.isArray(noMatchValue) && noMatchValue.includes(value)) {
                match = false;
            }
            else if (Array.isArray(matchValue) && Array.isArray(value) && matchValue.some(item => value.includes(item))) {
                match = true;
            }
            else if (Array.isArray(noMatchValue) && Array.isArray(value) && noMatchValue.some(item => value.includes(item))) {
                match = false;
            }
            else if (typeof value === "number") {
                if (value === 0) {
                    match = false;
                }
                else if (value === 1) {
                    match = true;
                }
            }
            else if (typeof value === "string") {
                // When there are no values supplied to compare the current value with, try to interpret the value as a boolean.
                if (matchValue === null || noMatchValue === null) {
                    match = value === "true" || value === "True" || value === "false" || value === "False" ?
                        value === "true" || value === "True" :
                        null;
                }
            }

            return match;
        }
    });
})(jQuery, window, document);