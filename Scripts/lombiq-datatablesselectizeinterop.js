/**
 * @summary     Lombiq - Datatables Selectize Interop
 * @description Workaround for Selectize in Datatables: The jQuery DataTable render logic is building a new element from provided HTML. 
 * This means that, although we attach a Selectize Editor plugin to an element in the DataTable, it's lost as the jQuery object is not used to append the new content.
 * @version     1.0
 * @file        lombiq-datatablesselectizeinterop.js
 * @author      Lombiq Technologies Ltd.
 */

$(document).on("init.dt", function (event) {
    $(event.target).find(".selectizeDropdownEditor input").each(function () {
        $(this).lombiq_SelectizeEditor();
    });
});