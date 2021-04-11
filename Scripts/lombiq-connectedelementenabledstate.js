/**
 * @summary     Lombiq - Connected Element Enabled State
 * @description Initializes a component that can automatically enable and disable other elements based on its current value.
 * @version     1.0
 * @file        lombiq-connectedelementenabledstate.js
 * @author      Lombiq Technologies Ltd.
 */

; (function ($, window, document, undefined) {
    "use strict";

    var pluginName = "lombiq_ConnectedElementEnabledState";

    var defaults = {
        initialValue: undefined,
        emptyNotEmptyMode: false,
        valueEnable: null,
        valueDisable: null,
        targetSelector: "",
        inverseTargetSelector: ""
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

            $(plugin.element).change(function (event, value) {
                plugin.refresh(value);
            });

            plugin.refresh(plugin.settings.initialValue);
        },

        isValueValid: function (value) {
            if (typeof value === "undefined" || value === null || value === "") {
                return false;
            }

            return true;
        },

        refresh: function (value) {
            var plugin = this;
            var enable = null;

            if (typeof value === "undefined") {
                value = plugin.element.val(); // If the provided value is not valid, try the element value.
            }

            if (plugin.settings.emptyNotEmptyMode && typeof value !== "undefined") {
                enable = true;
            }
            else {
                enable = $(document).dynamicComparer(value, plugin.settings.valueEnable, plugin.settings.valueDisable);
            }

            var target = $(plugin.settings.targetSelector).not(plugin.element);
            var inverseTarget = $(plugin.settings.inverseTargetSelector).not(plugin.element);

            if (enable === null) {
                target.disable();
                if (plugin.settings.inverseTargetSelector) {
                    inverseTarget.disable();
                }
            }
            else if (enable) {
                target.enable();
                if (plugin.settings.inverseTargetSelector) {
                    inverseTarget.disable();
                }
            }
            else {
                target.disable();
                if (plugin.settings.inverseTargetSelector) {
                    inverseTarget.enable();
                }
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