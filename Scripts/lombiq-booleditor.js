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
        onChangedSelectorId: "",
        radioButtonTrueId: "",
        radioButtonsClass: "",
        checkboxButtonClass: "",
        checkboxClass: "",
        booleanFieldClass: ".",
        switchClass: ".",
        switcherClass: "",
        switchClassName: "",
        enabledModifierClassName: "",
        onSwitchClassname: "",
        offSwitchClassname: "",
        onSwitchClass: "",
        offSwitchClass: ""
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
                    $(plugin.settings.onChangedSelectorId + " " + plugin.settings.radioButtonsClass).on("click", function (event) {

                        var boolEditorValue = $(this).find(plugin.settings.radioButtonTrueId).is(":checked");

                        $(plugin.settings.onChangedSelectorId).trigger("change", [boolEditorValue]);
                    });
                    break;
                case "Toggle":
                    $(plugin.settings.switcherClass).on("click", function (event) {
                        // For some reason, the click event is triggered twice when rendered in a modal window.
                        event.stopImmediatePropagation();

                        var booleanField = $(this).siblings(plugin.settings.booleanFieldClass),
                            val = booleanField.val();

                        booleanField.val(val === "True" ? "False" : "True").trigger("change");

                        $(plugin.settings.onChangedSelectorId).trigger("change", [val.toLowerCase() == "false"]);

                        $(this).children(plugin.settings.switchClass).toggleClass(plugin.settings.switchClassName + plugin.settings.enabledModifierClassName);
                        $(this).children(plugin.settings.onSwitchClass).toggleClass(plugin.settings.onSwitchClassname + plugin.settings.enabledModifierClassName);
                        $(this).children(plugin.settings.offSwitchClass).toggleClass(plugin.settings.offSwitchClassname + plugin.settings.enabledModifierClassName);
                    });

                    break;
                case "Checkbox":
                    $(plugin.settings.onChangedSelectorId + " " + plugin.settings.checkboxButtonClass).on("click", function (event) {
                        var boolEditorValue = $(this).find(plugin.settings.checkboxClass).is(":checked");

                        $(plugin.settings.onChangedSelectorId).trigger("change", [boolEditorValue]);
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