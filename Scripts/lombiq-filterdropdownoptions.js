/**
 * @summary     Lombiq - Filter Dropdown Options
 * @description Filters a <select>'s <option> tags based on the selected option of the parent <select>
 * @version     1.0
 * @file        lombiq-filterdropdownoptions.js
 * @author      Lombiq Technologies Ltd.
 */

; (function ($, window, document, undefined) {
    "use strict";

    var pluginName = "lombiq_FilterDropdownOptions";

    var defaults = {
        parentDropdownName: "",
        valueStructures: ""
    };
    
    function Plugin(element, options) {
        this.element = element;
        this.settings = $.extend(true, {}, defaults, options);
        this._defaults = defaults;
        this._name = pluginName;

        this.init();
    }

    $.extend(Plugin.prototype, {
        init: function () {
            var plugin = this;
            var parentSelect = "select[name=" + plugin.settings.parentDropdownName + "]";

            $(parentSelect).change(function () {
                $(plugin.element).empty().data("options");
                let currentOptions = plugin.settings.valueStructures[$(parentSelect).val()];
                $.each(currentOptions, function (i) {
                    let option = currentOptions[i];
                    $(plugin.element).append($("<option>").text(option.name).val(option.value));
                });
            });
        }
    });
    
    $.fn[pluginName] = function (options) {
        return this.each(function () {
            if (!$.data(this, "plugin_" + pluginName)) {
                $.data(this, "plugin_" + pluginName, new Plugin(this, options));
            }
        });
    };
})(jQuery, window, document);