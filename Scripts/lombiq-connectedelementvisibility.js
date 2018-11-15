/**
 * @summary     Lombiq - Connected Element Visibility
 * @description Initializes a component that can automatically show and hide other elements based on its current value.
 * @version     1.0
 * @file        lombiq-connectedelementvisibility.js
 * @author      Lombiq Technologies Ltd.
 */

; (function ($, window, document, undefined) {
    "use strict";

    var pluginName = "lombiq_ConnectedElementVisibility";

    var defaults = {
        initialValue: "false",
        valueShow: null,
        valueHide: null,
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

            plugin.updateVisibility(plugin.settings.initialValue);

            $(plugin.element).on("change", function (event, value) {
                plugin.updateVisibility(value);
            });
        },

        updateVisibility: function (value) {
            if (typeof value === "undefined") return;

            var plugin = this;

            var show = null;
            if (typeof value === "boolean") {
                show = value;
            }
            else if (typeof value === "number") {
                if (number === 0) {
                    show = false;
                }
                else if (number === 1) {
                    show = true;
                }
            }
            else if (typeof value === "string") {
                // When there are no values supplied to compare the current value with, try to interpret the value as a boolean.
                if (plugin.settings.valueShow === null || plugin.settings.valueHide === null) {
                    show = value === "true" || value === "True" || value === "false" || value === "False" ?
                        value === "true" || value === "True" :
                        null;
                }
                else if (value === plugin.settings.valueShow) {
                    show = true;
                }
                else if (value === plugin.settings.valueHide) {
                    show = false;
                }
            }

            var validationAttributes = ["required", "min", "max", "pattern"];
            var replaceRequiredAttribute = function (selector) {
                $.each(validationAttributes, function () {
                    $(document).replaceElementAttribute(selector, this, this + "-hidden");
                });
            };

            var replaceRequiredHiddenAttribute = function (selector) {
                $.each(validationAttributes, function () {
                    $(document).replaceElementAttribute(selector, this + "-hidden", this);
                });
            };

            if (show === null) {
                $(plugin.settings.targetSelector).hide();
                replaceRequiredAttribute(plugin.settings.targetSelector);
                if (plugin.settings.inverseTargetSelector) {
                    $(plugin.settings.inverseTargetSelector).hide();
                    replaceRequiredAttribute(plugin.settings.inverseTargetSelector);
                }
            }
            else if (show) {
                $(plugin.settings.targetSelector).show();
                replaceRequiredHiddenAttribute(plugin.settings.targetSelector);
                if (plugin.settings.inverseTargetSelector) {
                    $(plugin.settings.inverseTargetSelector).hide();
                    replaceRequiredAttribute(plugin.settings.inverseTargetSelector);
                }
            }
            else {
                $(plugin.settings.targetSelector).hide();
                replaceRequiredAttribute(plugin.settings.targetSelector);
                if (plugin.settings.inverseTargetSelector) {
                    $(plugin.settings.inverseTargetSelector).show();
                    replaceRequiredHiddenAttribute(plugin.settings.inverseTargetSelector);
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