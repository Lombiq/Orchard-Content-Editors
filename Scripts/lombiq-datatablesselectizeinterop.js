/**
 * @summary     Lombiq - Datatables Selectize Interop
 * @description Workaround for Selectize in Datatables: The selectize input forgets its jQuery object on Datatable initialisation,
 * hence it has to be reinitialized.
 * @version     1.0
 * @file        lombiq-datatablesselectizeinterop.js
 * @author      Lombiq Technologies Ltd.
 */

$(document).on("lombiq_DataTables_OnInitComplete", function (plugin) {
    $(plugin.target).find(".selectizeDropdownEditor input").each(function () {
        $(this).lombiq_SelectizeEditor();
    });
});