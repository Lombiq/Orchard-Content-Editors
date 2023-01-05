/* Based on https://www.billerickson.net/code/hide-empty-fields-get-form/.
 * Calling this function on a form element will disable all of its input elements with an empty value
 * upon submission so that they don't show up in the query string.
 * Exceptions can be declared by adding the "dontdisableifempty" attribute to an input element.
 */
; (function ($) {
    $.fn.disableEmptyFormInputs = function () {
        this.each(function () {
            var element = $(this);

            if (element.prop("tagName") !== "FORM") return;

            element.on("submit", function () {
                element.find(":input:not([dontdisableifempty])").filter(function () {
                    return !this.value;
                }).attr("disabled", "disabled");

                return true; // To ensure that the form still submits.
            });

            // Un-disable form fields when page loads, in case they click back after submission.
            element.find(":input:not([dontdisableifempty])").filter(function () {
                return !this.value;
            }).prop("disabled", false);
        });
    };
}(jQuery));