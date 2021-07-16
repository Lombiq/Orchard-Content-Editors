; (function ($, window, document, undefined) {
    $(function () {
        var closeButton = $(this).find(".modalCloseButton");
        if (closeButton.length) {
            closeButton.click(function () {
                closeButton.closest(".modal").dialog("close");
            });
        }
    });
})(jQuery, window, document);