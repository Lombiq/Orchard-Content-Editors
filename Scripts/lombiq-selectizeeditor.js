/**
 * @summary     Lombiq - Selectize Editor
 * @description Initializes a wrapper for a Selectize component.
 * @version     1.0
 * @file        lombiq-selectizeeditor.js
 * @author      Lombiq Technologies Ltd.
 */

; (function ($, window, document, undefined) {
    "use strict";

    var pluginName = "lombiq_SelectizeEditor";

    var defaults = {
        singleChoice: false,
        maxItems: null,
        selectizeOptions: new Array(),
        selectizeSelectedOptions: new Array(),
    };

    function Plugin(element, settings) {
        this.element = element;
        this.settings = $.extend(true, {}, defaults, settings);
        this._defaults = defaults;
        this._name = pluginName;

        if (this.settings.maxItems == null && this.settings.singleChoice) {
            this.settings.maxItems = 1;
        }

        this.init();
    }

    $.extend(Plugin.prototype, {
        init: function (settings) {
            var plugin = this;

            var $selectizeElement = plugin.element.selectize({ "maxItems": plugin.settings.maxItems })[0].selectize;

            for (var i = 0; i < plugin.settings.selectizeOptions.length; i++) {
                $selectizeElement.addOption({
                    value: plugin.settings.selectizeOptions[i].Value,
                    text: plugin.settings.selectizeOptions[i].Text
                });
            }

            for (var i = 0; i < plugin.settings.selectizeSelectedOptions.length; i++) {
                $selectizeElement.addItem(plugin.settings.selectizeSelectedOptions[i].Value);
            }
        }
    });

    $.fn[pluginName] = function (settings) {
        // Return null if the element query is invalid.
        if (!this || this.length === 0) return null;

        // "map" makes it possible to return the already existing or currently initialized plugin instances.
        return this.map(function () {

            var dataAttributeName = "data-lombiqselectizeeditorsettings";

            // If settings is defined, then we'll save it in a data attribute of the element
            // to make delayed reinitialization possible later.
            if (settings) {
                $(this).attr(dataAttributeName, JSON.stringify(settings));
            }
            // If settings is undefined...
            else {
                // ... then try to grab settings from a previous initialization.
                var settingsData = $(this).attr(dataAttributeName);

                if (settingsData) {
                    // Revert the changes made by Selectize to the element.
                    $(this).removeClass("selectized");
                    $(this).removeAttr("style");
                    $(this).val("");
                    $(this).removeAttr("tabindex");
                    $(this).siblings(".selectize-control").remove();

                    // Destroy the existing Selectize element.
                    var exisingSelectize = $(this).selectize();
                    if (typeof (exisingSelectize) !== "undefined" && exisingSelectize.length > 0) {
                        exisingSelectize[0].selectize.destroy();
                    }

                    // Use the settings data to construct the settings.
                    settings = JSON.parse(settingsData);
                }
            }

            // ... then create a plugin instance ...
            $.data(this, "plugin_" + pluginName, new Plugin($(this), settings));

            // Return the plugin instance (if it exists), undefined otherwise.
            return $.data(this, "plugin_" + pluginName);
        });
    };
})(jQuery, window, document);