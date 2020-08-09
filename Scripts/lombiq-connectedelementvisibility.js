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
        instanceName: null,
        initialValue: null,
        valueFunction: function (element) { return element.val(); },
        valueShow: null,
        valueHide: null,
        targetSelector: "",
        inverseTargetSelector: "",
        hideDefault: true,
        clearTargetInputsOnHide: false,
        visibilityChangedCallback: function () { }
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

            if (plugin.settings.initialValue !== null) {
                plugin.refresh(plugin.settings.initialValue);
            }

            $(plugin.element).change(function (event, value) {
                plugin.refresh(value);
            });
        },

        isValueValid: function (value) {
            return !(typeof value === "undefined" || value === null || value === "");
        },

        refresh: function (value) {
            var plugin = this;

            if (!plugin.isValueValid(value)) {
                value = plugin.element.val(); // If the provided value is not valid, try the element value.

                if (!plugin.isValueValid(value)) {
                    value = plugin.settings.valueFunction(plugin.element); // If the element value is not valid, try the value function.

                    if (!plugin.isValueValid(value)) {
                        return; // If the value function's result is not valid, then we can't do anything.
                    }
                }
            }

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
            else if (Array.isArray(plugin.settings.valueShow) && plugin.settings.valueShow.includes(value)) {
                show = true;
            }
            else if (Array.isArray(plugin.settings.valueHide) && plugin.settings.valueHide.includes(value)) {
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

            if (show === null && plugin.settings.hideDefault) {
                target.hide();
                replaceRequiredAttribute(plugin.settings.targetSelector);
                if (plugin.settings.inverseTargetSelector) {
                    inverseTarget.hide();
                    replaceRequiredAttribute(plugin.settings.inverseTargetSelector);
                }
                if (plugin.settings.clearTargetInputsOnHide) {
                    target.find("input, textarea").val("");
                    target.find("select").prop("selectedIndex", 0);
                }
            }
            else if (show === null && !plugin.settings.hideDefault || show) {
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
                if (plugin.settings.clearTargetInputsOnHide) {
                    target.find("input, textarea").val("");
                    target.find("select").prop("selectedIndex", 0);
                }
            }

            plugin.settings.visibilityChangedCallback(target, show === null ? !plugin.settings.hideDefault : show);
        }
    });

    $.fn[pluginName] = function (options) {
        // Return null if the element query is invalid.
        if (!this || this.length === 0) return null;

        // If options is available and instance name is defined, we're using that too
        // to generate the name of this plugin instance, so multiple instances can be
        // attached to the same DOM element using unique instance names.
        var pluginInstanceName = "plugin_" + pluginName;
        if (options && options.instanceName) {
            pluginInstanceName += "." + options.instanceName;
        }

        // "map" makes it possible to return the already existing or currently initialized plugin instances.
        return this.map(function () {
            // If "options" is defined, but the plugin is not instantiated on this element ...
            if (options && !$.data(this, pluginInstanceName)) {
                // ... then create a plugin instance ...
                $.data(this, pluginInstanceName, new Plugin($(this), options));
            }

            // ... and then return the plugin instance, which might be null
            // if the plugin is not instantiated on this element and "options" is undefined.
            return $.data(this, pluginInstanceName);
        });
    };
})(jQuery, window, document);