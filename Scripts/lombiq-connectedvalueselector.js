/**
 * @summary     Lombiq - Connected Value Selector
 * @description Connects a value selector control with a parent control to be able to react
 *              to the parent changing its value by changing the set of selectable values.
 * @version     1.0
 * @file        lombiq-connectedvalueselector.js
 * @author      Lombiq Technologies Ltd.
 */

; (function ($, window, document, undefined) {
    "use strict";

    var pluginName = "lombiq_ConnectedValueSelector";

    var defaults = {
        parentElementSelector: "",
        childElementValueSelector: "option",
        valueHierarchy: "",
        hasDefaultEmptyValue: false,
        defaultEmptyValue: ""
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
            var parentElement = $(plugin.settings.parentElementSelector);

            var parentElementChanged = function () {
                var parentValue = parentElement.val();

                if (typeof parentValue === "undefined" || !parentValue || parentValue === 0 || parentValue === "") return;

                var currentValues = plugin.settings.valueHierarchy[parentValue];

                if (typeof currentValues === "undefined") return;

                $.each($(plugin.element).children(plugin.settings.childElementValueSelector), function () {
                    var currentElementValue = $(this).val();

                    if (currentValues[currentElementValue]) {
                        $(this).show();
                    }
                    // Don't hide the default empty value.
                    else if (!plugin.settings.hasDefaultEmptyValue || plugin.settings.defaultEmptyValue !== currentElementValue) {
                        $(this).hide();
                        $(this).prop("selected", false);
                    }
                });
            };

            if (parentElement) {
                // Initialize with selectable children filtered.
                parentElementChanged();

                $(parentElement).change(parentElementChanged);
            }
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