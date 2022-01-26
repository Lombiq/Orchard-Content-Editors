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
                element.find(":input").filter(function () {
                    return !this.value && !$(this).is("[dontdisableifempty]");
                }).attr("disabled", "disabled");

                return true; // To ensure that the form still submits.
            });
        });
    };
}(jQuery));