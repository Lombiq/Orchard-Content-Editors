/**
 * @summary     Checkbox list editor
 * @description Checkbox list editor functionality with Select All option and search filter.
 * @version     1.0
 * @file        lombiq-checkboxlistfield.js
 * @author      Lombiq Technologies Ltd.
 */

; (function ($, window, document, undefined) {
    "use strict";

    var pluginName = "lombiq_CheckboxListEditor";

    var defaults = {
        searchFilterElementClass: "",
        searchFilterContainerElementClass: "",
        controlGroupElementClass: "",
        checkboxContainerSelector: "",
        checkboxInputElementClass: "",
        inputCheckboxInputElementSelector: "",
        checkboxSelectAllItemElementClass: "",
        selectAllElementClass: "",
        inputSelectAllElementSelector: "",
        checkboxItemElementClass: ""
    };

    function Plugin(element, options) {
        this.element = element;
        this.settings = $.extend(true, {}, defaults, options);
        this._defaults = defaults;
        this._name = pluginName;

        this.settings.isSearchEnabled = this.settings.searchFilterElementClass.length > 0 && this.settings.searchFilterContainerElementClass.length > 0;

        this.init();
    }

    $.extend(Plugin.prototype, {
        init: function () {
            var plugin = this;

            // Make jQuery :contains Case-Insensitive.
            $.expr[":"].contains = $.expr.createPseudo(function (arg) {
                return function (elem) {
                    return $(elem).text().toUpperCase().indexOf(arg.toUpperCase()) >= 0;
                };
            });

            if (plugin.settings.isSearchEnabled) {
                $(plugin.settings.searchFilterElementClass).change(function () {
                var filter = $(this).val();
                var controlGroup = $(this).parents(plugin.settings.searchFilterContainerElementClass).next(plugin.settings.controlGroupElementClass);

                if (filter.length === 0) {
                    controlGroup.find(plugin.settings.checkboxContainerSelector + ":hidden").show();
                    $(this).prev().removeClass("glyphicon-remove").addClass("glyphicon-search");
                    return;
                }
                else {
                    $(this).prev().removeClass("glyphicon-search").addClass("glyphicon-remove");

                    $(".glyphicon-remove").click(function () {
                        $(this).next().val("");
                        $(this).removeClass("glyphicon-remove").addClass("glyphicon-search");
                        controlGroup.find(plugin.settings.checkboxItemElementClass + ":hidden").show();
                    });
                }

                controlGroup.find(plugin.settings.checkboxContainerSelector + ":contains('" + filter + "'):hidden").show();

                var $itemsToHide = controlGroup.find(plugin.settings.checkboxContainerSelector + ":not(:contains('" + filter + "')):visible");
                $itemsToHide.children(plugin.settings.checkboxInputElementClass).prop("checked", false);
                $itemsToHide.hide();

                }).on("keyup search", function () {
                    $(this).change();
                    reevaluateSelectAllState($(this).parents(plugin.settings.searchFilterContainerElementClass).next(plugin.settings.controlGroupElementClass));
                });
            }

            var $checkboxes = $(plugin.settings.checkboxInputElementClass);
            $checkboxes.on("change", function () {
                reevaluateSelectAllState($(this).parent().parent());
            });

            var reevaluateSelectAllState = function ($controlGroup) {
                var $selectAll = $controlGroup.children(plugin.settings.checkboxSelectAllItemElementClass);
                var selectAllInput = $selectAll.children(plugin.settings.inputSelectAllElementSelector);
                var $visibleCheckboxes = $controlGroup.children(plugin.settings.checkboxItemElementClass + ":not(" +
                    plugin.settings.checkboxSelectAllItemElementClass + ")").children(plugin.settings.inputCheckboxInputElementSelector + ":visible");

                if ($visibleCheckboxes.length === 0) {
                    $selectAll.hide();
                }
                else {
                    $selectAll.show();
                    selectAllInput.prop("checked", $visibleCheckboxes.filter(":not(:checked)").length === 0);
                }
            };

            var selectAll = $(plugin.settings.selectAllElementClass);
            selectAll.change(function () {
                var checked = this.checked;
                $(this).parent().siblings(plugin.settings.checkboxContainerSelector).children(plugin.settings.checkboxInputElementClass + ":visible").each(function () {
                    this.checked = checked;
                });
            });
        },

        clear: function () {
            var plugin = this;
            var element = $(plugin.element);
            var $controlGroup = element.find(plugin.settings.controlGroupElementClass);

            if (plugin.settings.isSearchEnabled) {
                element.find(plugin.settings.searchFilterElementClass).val([]).keyup();
            }

            $controlGroup.find(plugin.settings.checkboxInputElementClass).filter(":checked").prop("checked", false);

            return plugin;
        },

        val: function () {
            return $(this.settings.checkboxInputElementClass).filter(":checked:not(:disabled)").map(function () {
                return $(this).val();
            }).toArray();
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
                $.data(this, "plugin_" + pluginName, new Plugin(this, options));
            }

            // ... and then return the plugin instance, which might be null
            // if the plugin is not instantiated on this element and "options" is undefined.
            return $.data(this, "plugin_" + pluginName);
        });
    };

    $.fn.originalVal = $.fn.val;
    $.fn.val = function (value) {
        var plugin = $(this).lombiq_CheckboxListEditor()[0];

        return plugin ? plugin.val() : value ? $(this).originalVal(value) : $(this).originalVal();
    };

    $.fn.originalEnable = $.fn.enable;
    $.fn.enable = function () {
        $(this).originalEnable();

        var plugin = $(this).lombiq_CheckboxListEditor()[0];

        if (plugin) {
            $(this).find(plugin.settings.checkboxItemElementClass).removeClass("disabled");
        }
    };

    $.fn.originalDisable = $.fn.disable;
    $.fn.disable = function () {
        $(this).originalDisable();

        var plugin = $(this).lombiq_CheckboxListEditor()[0];

        if (plugin) {
            $(this).find(plugin.settings.checkboxItemElementClass).addClass("disabled");
        }
    };
})(jQuery, window, document);