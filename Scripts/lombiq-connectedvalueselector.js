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
                if (typeof parentElement === "undefined" || parentElement.length === 0) return;

                var parentValue = parentElement.val();

                if (typeof parentValue === "undefined") return;

                var childElements = $(plugin.element).find(plugin.settings.childElementValueSelector);

                if (childElements.length > 0) {
                    var currentValues = [];
                    if (Array.isArray(parentValue)) {
                        $.each(parentValue, function (index, value) {
                            if ($.inArray(value, plugin.settings.valueHierarchy)) {
                                $.merge(currentValues, plugin.settings.valueHierarchy[value]);
                            }
                        });
                    }
                    else {
                        currentValues = plugin.settings.valueHierarchy[parentValue];
                    }

                    $.each(childElements, function () {
                        var currentElement = $(this);
                        var currentElementTag = $(currentElement).prop("tagName").toLowerCase();
                        var hierarchy = currentElementTag !== "input" && currentElementTag !== "option";

                        // If the current element is not an input or option, then it defines a hierarchy of elements,
                        // in which case we need to find the element among its children that supplies the value.
                        var valueElement = hierarchy ? $(currentElement).find("input,option") : $(currentElement);

                        var value = valueElement.val();

                        if ($.inArray(value, currentValues) > -1) {
                            $(currentElement).show();
                        }
                        // Don't hide the default empty value.
                        else if (!plugin.settings.hasDefaultEmptyValue || plugin.settings.defaultEmptyValue !== value) {
                            valueElement.prop("selected", false).prop("checked", false);
                            $(currentElement).hide();
                        }
                    });
                }

                $(plugin.element).trigger("change");
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