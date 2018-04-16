/**
 * @summary     Lombiq - Bool Editor
 * @description Initializes Bool Editors based on render mode and extending jQuery with ShowHideClassByBoolEditorId.
 * @version     1.0
 * @file        lombiq-booleditor.js
 * @author      Lombiq Technologies Ltd.
 */

; (function ($, window, document, undefined) {
    "use strict";

    var pluginName = "lombiq_BoolEditor";

    var defaults = {
        renderMode: "",
        radioButtonsSettings: {
            onChangedSelectorId: "",
            radioButtonTrueId: "", // If this checked the Booleditor is true.
            radioButtonsClass: "" // The wrapper of the radio buttons.
        },
        checkboxSettings: {
            onChangedSelectorId: "",
            checkboxButtonClass: "", // The wrapper of the checkbox and label.
            checkboxClass: "", // The class of the checkbox.
        },
        toggleSettings: {
            booleanFieldClass: "",
            textTrue: "",
            textFalse: ""
        }
    }

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

            switch (plugin.settings.renderMode) {
                case "RadioButtons":
                    $(plugin.settings.radioButtonsSettings.onChangedSelectorId + " " + plugin.settings.radioButtonsSettings.radioButtonsClass).on("click", function (event) {
                        var boolEditorValue = $(this).find(plugin.settings.radioButtonsSettings.radioButtonTrueId).is(":checked");

                        $(plugin.settings.radioButtonsSettings.onChangedSelectorId).trigger("change", [boolEditorValue]);
                    });
                    break;
                case "Toggle":
                    $(plugin.settings.toggleSettings.booleanFieldClass).lc_switch(plugin.settings.toggleSettings.textTrue, plugin.settings.toggleSettings.textFalse);
                    break;
                case "Checkbox":
                    $(plugin.settings.checkboxSettings.onChangedSelectorId + " " + plugin.settings.checkboxSettings.checkboxButtonClass).on("click", function (event) {
                        var boolEditorValue = $(this).find(plugin.settings.checkboxSettings.checkboxClass).is(":checked");

                        $(plugin.settings.checkboxSettings.onChangedSelectorId).trigger("change", [boolEditorValue]);
                    });
                    break;
                default:
                    return;
            }
        }
    });

    $.fn[pluginName] = function (options) {
        // Return null if the element query is invalid.
        if (!this || this.length == 0) return null;

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

    $(function () {
        $.fn.extend({
            ShowHideClassByBoolEditorId: function (targetClass, inverseTargetClass) {
                $("#" + this.selector).on("change", function (e, boolEditorValue) {
                    if (boolEditorValue == null) return;

                    if (boolEditorValue) {
                        $("." + targetClass).show();
                        if (inverseTargetClass) $("." + inverseTargetClass).hide();
                    }
                    else {
                        $("." + targetClass).hide();
                        if (inverseTargetClass) $("." + inverseTargetClass).show();
                    }
                });
            }
        });
    });
})(jQuery, window, document);