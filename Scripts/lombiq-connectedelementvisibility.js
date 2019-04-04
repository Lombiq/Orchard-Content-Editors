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
        initialValue: null,
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

            if (!plugin.settings.initialValue) {
                plugin.settings.initialValue = $(plugin.element).val();
            }

            plugin.updateVisibility(plugin.settings.initialValue);

            $(plugin.element).on("change", function (event, value) {
                var actualValue = null;
                if (plugin.isValueValid(value)) {
                    actualValue = value;
                }
                else if (plugin.isValueValid($(this).val())) {
                    actualValue = $(this).val();
                }

                plugin.updateVisibility(actualValue);
            });
        },

        isValueValid: function (value) {
            if (typeof value === "undefined" || value === null || value === "") {
                return false;
            }

            return true;
        },

        updateVisibility: function (value) {
            var plugin = this;

            if (!plugin.isValueValid(value)) return;

            var show = null;
            if (typeof value === "boolean") {
                show = value;
            }
            else if (plugin.settings.valueShow !== null && value === plugin.settings.valueShow) {
                show = true;
            }
            else if (plugin.settings.valueHide !== null && value === plugin.settings.valueHide) {
                show = false;
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

            var target = $(plugin.settings.targetSelector).not(plugin.element);
            var inverseTarget = $(plugin.settings.inverseTargetSelector).not(plugin.element);

            if (show === null) {
                target.hide();
                replaceRequiredAttribute(plugin.settings.targetSelector);
                if (plugin.settings.inverseTargetSelector) {
                    inverseTarget.hide();
                    replaceRequiredAttribute(plugin.settings.inverseTargetSelector);
                }
            }
            else if (show) {
                target.show();
                replaceRequiredHiddenAttribute(plugin.settings.targetSelector);
                if (plugin.settings.inverseTargetSelector) {
                    inverseTarget.hide();
                    replaceRequiredAttribute(plugin.settings.inverseTargetSelector);
                }
            }
            else {
                target.hide();
                replaceRequiredAttribute(plugin.settings.targetSelector);
                if (plugin.settings.inverseTargetSelector) {
                    inverseTarget.show();
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