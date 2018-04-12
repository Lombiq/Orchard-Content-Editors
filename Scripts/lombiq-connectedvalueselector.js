﻿/**
 * @summary     Lombiq - Connected Value Selector
 * @description Connects a value selector control with a parent control to be able to react
 *              to the parent changing its value by changing the set of selectable values.
 *              Currently only supports select-option structures on both end.
 * @version     1.0
 * @file        lombiq-connectedvalueselector.js
 * @author      Lombiq Technologies Ltd.
 */

; (function ($, window, document, undefined) {
    "use strict";

    var pluginName = "lombiq_ConnectedValueSelector";

    var defaults = {
        parentElementName: "",
        valueHierarchy: ""
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
            var parentElement = "select[name='" + plugin.settings.parentElementName + "']";

            var parentElementChanged = function () {
                var selectedValue = $(plugin.element).val();
                $(plugin.element).empty().data("options");

                var parentValue = $(parentElement).val();
                var currentValues = plugin.settings.valueHierarchy[parentValue];

                $.each(Object.keys(currentValues), function () {
                    var optionTag = $("<option>").text(currentValues[this]).val(this);
                    if (selectedValue === this) {
                        optionTag.attr("selected", "selected");
                    }
                    $(plugin.element).append(optionTag);
                });
            };

            if (parentElement) {
                // Initially filter child dropdown options
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