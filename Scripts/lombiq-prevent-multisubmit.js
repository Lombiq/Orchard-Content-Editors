; (function ($, window, document, undefined) {
    $.fn.preventMultisubmit = function () {
        const submittingClass = "submitting";
        form = $(this);
        form.on("submit", function (e) {
            if (form.hasClass(submittingClass)) {
                e.preventDefault();
                return;
            }
            form.addClass(submittingClass);
            $.lombiq_LoadingIndicator.show();
        });        
    };
})(jQuery, window, document);