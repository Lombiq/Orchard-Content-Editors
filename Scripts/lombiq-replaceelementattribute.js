; (function ($, window, document, undefined) {
    $.fn.extend({
        replaceElementAttribute: function (selector, attribute, newAttribute) {
            $(selector).find("[" + attribute + "]").each(function () {
                var $element = $(this);
                var attributeValue = $element.attr(attribute);
                if (jQuery.expr.match.bool.test(attribute)) {
                    $element.prop(attr, false);
                } else {
                    $element.removeAttr(attribute);
                }
                $element.attr(newAttribute, attributeValue);
            });
        }
    });
})(jQuery, window, document);