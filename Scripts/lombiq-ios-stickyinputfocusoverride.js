/**
 * @summary     Lombiq - iOS Sticky input focus override
 * @description Workaround for iOS: Focused input elements don't lose focus when a touch event occurs outside the element (by (bad) design).
 * @version     1.0
 * @file        lombiq-ios-stickyinputfocusoverride.js
 * @author      Lombiq Technologies Ltd.
 */

$(document).on("touchstart", function (event) {
    if ($(":focus").is("input") && $(event.target).closest(".editorField").length == 0) {
        $(":focus").trigger("blur");
    }
});