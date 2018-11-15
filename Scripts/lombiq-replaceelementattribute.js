; (function ($, window, document, undefined) {
    $(function () {
        $.fn.extend({
            replaceElementAttribute: function (selector, attribute, newAttribute) {
                $(selector).find("[" + attribute + "]").each(function () {
                    var $self = $(this);
                    var attrValue = $self.attr(attribute);
                    $self.removeAttr(attribute);
                    $self.attr(newAttribute, attrValue);
                });
            }
        });
    });
})(jQuery, window, document);