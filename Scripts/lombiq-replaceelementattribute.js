; (function ($, window, document, undefined) {
    $.fn.extend({
        replaceElementAttribute: function (selector, attribute, newAttribute) {
            $(selector).find("[" + attribute + "]").each(function () {
                var $element = $(this);
                var attributeValue = $element.attr(attribute);
                $element.removeAttr(attribute);
                $element.attr(newAttribute, attributeValue);
            });
        }
    });
})(jQuery, window, document);