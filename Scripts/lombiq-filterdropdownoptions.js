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
            var parentSelect = "select[name='" + plugin.settings.parentDropdownName + "']";

            $(parentSelect).change(function () {
                $(plugin.element).empty().data("options");
                let selectedValue = $(parentSelect).val();
                let currentOptions = plugin.settings.valueStructures[selectedValue];
                $.each(currentOptions, function (i) {
                    let option = currentOptions[i];
                    $(plugin.element).append($("<option>").text(option.Name).val(option.Value));
                });
            });
        }
    });
    
    $.fn[pluginName] = function (options) {
        // Return null if the element query is invalid.
        if (!this || this.length === 0) return null;

        // "map" makes it possible to return the already existing or currently initialized plugin instances.
        return this.map(function () {
            // If "options" is defined, but the plugin is not instantiated on this element ...
            if (options && !$.data(this, "plugin_" + pluginName)) {
                // ... then create a plugin instance ...
                $.data(this, "plugin_" + pluginName, new Plugin($(this), options));
            }

            // ... and then return the plugin instance, which might be null
            // if the plugin is not instantiated on this element and "options" is undefined.
            return $.data(this, "plugin_" + pluginName);
        });
    };
})(jQuery, window, document);