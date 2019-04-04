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
        initialValue: null,
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

            plugin.updateState(plugin.settings.initialValue);

            $(plugin.element).on("change", function (event, value) {
                var actualValue = null;
                if (plugin.isValueValid(value)) {
                    actualValue = value;
                }
                else if (plugin.isValueValid($(this).val())) {
                    actualValue = $(this).val();
                }

                plugin.updateState(actualValue);
            });
        },

        isValueValid: function (value) {
            if (typeof value === "undefined" || value === null || value === "") {
                return false;
            }

            return true;
        },

        updateState: function (value) {
            var plugin = this;
            var enable = null;

            if (plugin.settings.emptyNotEmptyMode) {
                enable = plugin.isValueValid(value);
            }
            else {
                if (!plugin.isValueValid(value)) return;

                if (typeof value === "boolean") {
                    enable = value;
                }
                else if (plugin.settings.valueEnable !== null && value === plugin.settings.valueEnable) {
                    enable = true;
                }
                else if (plugin.settings.valueDisable !== null && value === plugin.settings.valueDisable) {
                    enable = false;
                }
                else if (typeof value === "number") {
                    if (number === 0) {
                        enable = false;
                    }
                    else if (number === 1) {
                        enable = true;
                    }
                }
                else if (typeof value === "string") {
                    // When there are no values supplied to compare the current value with, try to interpret the value as a boolean.
                    if (plugin.settings.valueEnable === null || plugin.settings.valueDisable === null) {
                        enable = value === "true" || value === "True" || value === "false" || value === "False" ?
                            value === "true" || value === "True" :
                            null;
                    }
                }
            }

            if (enable === null) {
                $(plugin.settings.targetSelector).not(plugin.element).disable();
                if (plugin.settings.inverseTargetSelector) {
                    $(plugin.settings.inverseTargetSelector).not(plugin.element).disable();
                }
            }
            else if (enable) {
                $(plugin.settings.targetSelector).not(plugin.element).enable();
                if (plugin.settings.inverseTargetSelector) {
                    $(plugin.settings.inverseTargetSelector).not(plugin.element).disable();
                }
            }
            else {
                $(plugin.settings.targetSelector).not(plugin.element).disable();
                if (plugin.settings.inverseTargetSelector) {
                    $(plugin.settings.inverseTargetSelector).not(plugin.element).enable();
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