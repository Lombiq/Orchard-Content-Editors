/**
 * @summary     Lombiq - DataTables Selectize Interop
 * @description Workaround for Selectize in DataTables: Since DataTables renders raw HTML, the jQuery plugin instances
 *              attached to elements inside a cell are lost. The SelectizeEditor plugin supports reinitialization, so
 *              the interop's job is to find those elements and trigger the reinitialization of the plugin instances
 *              found inside every DataTable instance when the given DataTable finished initialization.
 * @version     1.0
 * @file        lombiq-datatablesselectizeinterop.js
 * @author      Lombiq Technologies Ltd.
 */

$(document).on("init.dt", function (event) {
    $(event.target).find(".selectizeDropdownEditor input").each(function () {
        $(this).lombiq_SelectizeEditor();
    });
});