/**
 * @summary     Lombiq - Editors - DateTime Editor
 * @description Manages DateTime editor UI components.
 * @version     1.0
 * @file        lombiq-editors-datetimeeditor.js
 * @author      Lombiq Technologies Ltd.
 */


; (function ($, window, document, undefined) {
    "use strict";

    // See: https://stackoverflow.com/questions/24500726/replace-the-jquery-datepicker-dateformat-with-momentjs-parsing-and-format/24500727#24500727
    $.datepicker.parseDate = function (format, value) {
        return moment(value, format).toDate();
    };

    $.datepicker.formatDate = function (format, value) {
        return moment(value).format(format);
    };

    var pluginName = "lombiq_Editors_DateTimeEditor";

    var defaults = {
        isDateEditor: true,
        inputElementSelector: "",
        displayFormat: "MM/DD/YYYY",
        storeFormat: "YYYY-MM-DD",
        valueChanged: function (value) { },
        pickerOptions: {
            showButtonPanel: true,
            showOn: "button",
            buttonText: "<span class='icon-calendar'></span>"
        },
        errorMessages: {
            invalidFormatErrorText: "",
            valueIsLowerThanMinimumText: "",
            valueIsGreaterThanMaximumText: ""
        }
    };

    function Plugin(element, options) {
        this.element = element;
        this.settings = $.extend(true, {}, defaults, options);
        this.pickerElement = $(element);
        this.inputElement = $(this.settings.inputElementSelector);
        this._defaults = defaults;
        this._name = pluginName;

        this.init();
    }

    $.extend(Plugin.prototype, {
        init: function () {
            var plugin = this;

            if (plugin.settings.isDateEditor) {
                var pickerOptions = $.extend(true, {}, plugin.settings.pickerOptions, {
                    dateFormat: plugin.settings.displayFormat,
                    onSelect: function () {
                        plugin.setValue(plugin.pickerElement.datepicker("getDate"));
                    }
                });

                var minAttribute = plugin.pickerElement.attr("min");
                var maxAttribute = plugin.pickerElement.attr("max");
                if (typeof minAttribute !== "undefined" && minAttribute !== false) {
                    pickerOptions.minDate = moment(minAttribute).toDate();
                }
                if (typeof maxAttribute !== "undefined" && maxAttribute !== false) {
                    pickerOptions.maxDate = moment(maxAttribute).toDate();
                }

                plugin.pickerElement.datepicker(pickerOptions);
            }

            plugin.pickerElement.on("change", function () {
                plugin.setValue(plugin.pickerElement.val(), plugin.settings.displayFormat);
            });

            setTimeout(function () { plugin.setValue(plugin.pickerElement.val(), plugin.settings.displayFormat) }, 500);
        },

        showErrorAndRestore: function (message) {
            if (message && message.length > 0) {
                alert(message);
            }

            plugin.updateDisplayedValue();
        },

        getValue: function () {
            var plugin = this;

            var storeText = plugin.inputElement.val();

            return typeof storeText !== "undefined" && storeText.length > 0 ? plugin.pickerElement.datepicker("getDate") : null;
        },

        setValue: function (value, format) {
            var plugin = this;

            var momentValue = null;
            if (value) {
                if (typeof value === "string") {
                    momentValue = moment(value, format);
                }
                else {
                    momentValue = moment(value);
                }
            }

            if (plugin.isDateEditor) {
                if (momentValue === null || !momentValue.isValid()) {
                    return plugin.showErrorAndRestore(plugin.settings.errorMessages.invalidFormatErrorText);
                }

                var minDate = plugin.pickerElement.datepicker("option", "minDate");
                var maxDate = plugin.pickerElement.datepicker("option", "maxDate");

                if (maxDate !== null && date > maxDate) {
                    return showErrorAndRestore(plugin.settings.errorMessages.valueIsGreaterThanMaximumText);
                }

                if (minDate !== null && date < minDate) {
                    return showErrorAndRestore(plugin.settings.errorMessages.valueIsLowerThanMinimumText);
                }

                plugin.pickerElement.datepicker("setDate", momentValue);
            }

            plugin.updateStoredValue(momentValue);

            plugin.settings.valueChanged(momentValue);
        },

        updateStoredValue: function (date) {
            var plugin = this;

            if (!date) {
                date = plugin.getValue();
            }

            if (date !== null) {
                plugin.inputElement.val(moment(date).format(plugin.settings.storeFormat));
            }
            else {
                plugin.inputElement.val("");
            }
        },

        updateDisplayedValue: function (date) {
            var plugin = this;

            if (!date) {
                var storedValue = plugin.inputElement.val();
                date = storedValue ? moment(storedValue, plugin.settings.storeFormat).toDate() : null;
            }

            if (date !== null) {
                plugin.pickerElement.datepicker("setDate", date);

                if (!plugin.settings.isDateEditor) {
                    plugin.pickerElement.val(moment(date).format("HH:mm"));
                }
            }
            else {
                plugin.pickerElement.val("");
            }
        },

        updateMinValue: function (date) {
            var plugin = this;

            plugin.pickerElement.datepicker("option", "minDate", moment(date).toDate());
        },

        updateMaxValue: function (date) {
            var plugin = this;

            plugin.pickerElement.datepicker("option", "maxDate", moment(date).toDate());
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
})(jQuery, window, document);