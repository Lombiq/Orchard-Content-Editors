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
        initialValue: undefined,
        valueFunction: undefined,
        valueShow: undefined,
        valueHide: undefined,
        targetSelector: "",
        inverseTargetSelector: "",
        clearTargetInputsOnHide: false,
        visibilityChangedCallback: function () { }
    };

    function Plugin(element, options) {
        this.element = element;
        this.settings = $.extend(true, {}, defaults, options);
        this._defaults = defaults;
        this._name = pluginName;

        var pluginMarkerClass = "lombiq-ConnectedElementVisibility";
        $(element).addClass(pluginMarkerClass);
        this.settings.pluginMarkerSelector = "." + pluginMarkerClass;

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

        refresh: function (value) {
            var plugin = this;

            var show = $(document).dynamicComparer(value, plugin.settings.valueShow, plugin.settings.valueHide);

            if (show === null) {
                if (typeof plugin.settings.valueFunction !== "undefined") {
                    // If the element value is not valid, try the value function.
                    show = $(document).dynamicComparer(plugin.settings.valueFunction(plugin.element), plugin.settings.valueShow, plugin.settings.valueHide);
                }

                if (show === null) {
                    // If the provided value is not valid, try the element value.
                    show = $(document).dynamicComparer(plugin.element.val(), plugin.settings.valueShow, plugin.settings.valueHide);

                    if (show === null) {
                        return;
                    }
                }
            }

            // "refreshChildren" is needed so that the attributes of input elements inside child plugins
            // (i.e. plugins attached to elements whose parent is a target or an inverse target) are correctly set
            // after this plugin updates input elements in its own element tree.
            var refreshChildren = function () {
                var allTargetsSelector = [plugin.settings.targetSelector, plugin.settings.inverseTargetSelector].filter(Boolean).join(",");
                if (allTargetsSelector.length > 0) {
                    var allTargetsChildrenPlugins = $(allTargetsSelector)
                        .find(plugin.settings.pluginMarkerSelector)
                        .lombiq_ConnectedElementVisibility()
                        ?? new Array();

                    if (allTargetsChildrenPlugins.length > 0) {
                        $.each(allTargetsChildrenPlugins, function () {
                            this.refresh();
                        });
                    }
                }
            }

            var validationAttributes = ["required", "min", "max", "pattern"];
            var replaceValidationAttributes = function (selector) {
                $.each(validationAttributes, function () {
                    $(document).replaceElementAttribute(selector, this, this + "-hidden");
                    $(document).replaceElementAttribute(selector + " input:not([type=hidden])", this, this + "-hidden");
                });

                $(document).replaceElementAttribute(selector + " textarea", "required", "required-hidden");
                $(document).replaceElementAttribute(selector + " select", "required", "required-hidden");
            };
            var replaceHiddenValidationAttributes = function (selector) {
                $.each(validationAttributes, function () {
                    $(document).replaceElementAttribute(selector, this + "-hidden", this);
                    $(document).replaceElementAttribute(selector + " input:not([type=hidden])", this + "-hidden", this);
                });

                $(document).replaceElementAttribute(selector + " textarea", "required-hidden", "required");
                $(document).replaceElementAttribute(selector + " select", "required-hidden", "required");
            };

            var target = $(plugin.settings.targetSelector).not(plugin.element);
            var inverseTarget = $(plugin.settings.inverseTargetSelector).not(plugin.element);

            if (show === true) {
                target.show("slow");
                replaceHiddenValidationAttributes(plugin.settings.targetSelector);

                if (plugin.settings.inverseTargetSelector) {
                    inverseTarget.hide("slow");
                    replaceValidationAttributes(plugin.settings.inverseTargetSelector);
                }
            }
            else if (show === false) {
                target.hide("slow");
                replaceValidationAttributes(plugin.settings.targetSelector);

                if (plugin.settings.inverseTargetSelector) {
                    inverseTarget.show("slow");
                    replaceHiddenValidationAttributes(plugin.settings.inverseTargetSelector);
                }

                if (plugin.settings.clearTargetInputsOnHide) {
                    target.find("input, textarea").val("");
                    target.find("select").prop("selectedIndex", 0);
                    target.find("[type = radio]").removeAttr("checked").trigger("change", false);
                }
            }

            refreshChildren();

            plugin.settings.visibilityChangedCallback(target, show);
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