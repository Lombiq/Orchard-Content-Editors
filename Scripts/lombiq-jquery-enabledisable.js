/**
 * @summary     Enable and Disable functions for jQuery.
 * @description Enable and Disable functions for jQuery.
 * @version     1.0
 * @file        lombiq-jquery-enabledisable.js
 * @author      Lombiq Technologies Ltd.
 */

; (function ($, window, document, undefined) {
    $.fn.enable = function () {
        $(this).prop("disabled", false);
        $(this).find("input").prop("disabled", false);
    };

    $.fn.disable = function () {
        $(this).prop("disabled", true);
        $(this).find("input").prop("disabled", true);
    };
})(jQuery, window, document);