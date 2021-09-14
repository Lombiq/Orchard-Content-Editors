/**
 * @summary     Lombiq - DataTables ConnectedElementVisibility Interop
 * @description Workaround for ConnectedElementVisibility in DataTables: Since DataTables renders raw HTML, the jQuery
 *              plugin instances attached to elements inside a cell are lost. The ConnectedElementVisibility plugin
 *              supports reinitialization, so the interop's job is to find those elements and trigger the
 *              reinitialization of the plugin instances found inside every DataTable instance when the given DataTable
 *              finished initialization.
 * @version     1.0
 * @file        lombiq-datatables-connectedelementvisibility-interop.js
 * @author      Lombiq Technologies Ltd.
 */

$(document).on("init.dt", function (event) {
    $(event.target).find(".lombiq-ConnectedElementVisibility").each(function () {
        $(this).lombiq_ConnectedElementVisibility();
    });
});