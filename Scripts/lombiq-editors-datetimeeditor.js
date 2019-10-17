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

    function showErrorMessage(message) {
        if (message.length > 0) {
            alert(message);
        }
    }

    var pluginName = "lombiq_Editors_DateTimeEditor";

    var defaults = {
        inputElementSelector: "",
        dateDisplayFormat: "MM/DD/YYYY",
        dateStoreFormat: "YYYY-MM-DD",
        dateChanged: function (value) { },
        datepickerOptions: {
            showButtonPanel: true,
            showOn: "button",
            buttonImage: "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABcAAAAYCAYAAAARfGZ1AAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAA+5pVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuMy1jMDExIDY2LjE0NTY2MSwgMjAxMi8wMi8wNi0xNDo1NjoyNyAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wTU09Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9tbS8iIHhtbG5zOnN0UmVmPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvc1R5cGUvUmVzb3VyY2VSZWYjIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtbG5zOmRjPSJodHRwOi8vcHVybC5vcmcvZGMvZWxlbWVudHMvMS4xLyIgeG1wTU06T3JpZ2luYWxEb2N1bWVudElEPSJ1dWlkOjY1RTYzOTA2ODZDRjExREJBNkUyRDg4N0NFQUNCNDA3IiB4bXBNTTpEb2N1bWVudElEPSJ4bXAuZGlkOjg0M0YyRDVDMDY3QjExRTI5OUZEQTZGODg4RDc1ODdCIiB4bXBNTTpJbnN0YW5jZUlEPSJ4bXAuaWlkOjg0M0YyRDVCMDY3QjExRTI5OUZEQTZGODg4RDc1ODdCIiB4bXA6Q3JlYXRvclRvb2w9IkFkb2JlIFBob3Rvc2hvcCBDUzYgKE1hY2ludG9zaCkiPiA8eG1wTU06RGVyaXZlZEZyb20gc3RSZWY6aW5zdGFuY2VJRD0ieG1wLmlpZDowMTgwMTE3NDA3MjA2ODExODA4M0ZFMkJBM0M1RUU2NSIgc3RSZWY6ZG9jdW1lbnRJRD0ieG1wLmRpZDowNjgwMTE3NDA3MjA2ODExODA4M0U3NkRBMDNEMDVDMSIvPiA8ZGM6dGl0bGU+IDxyZGY6QWx0PiA8cmRmOmxpIHhtbDpsYW5nPSJ4LWRlZmF1bHQiPmdseXBoaWNvbnM8L3JkZjpsaT4gPC9yZGY6QWx0PiA8L2RjOnRpdGxlPiA8L3JkZjpEZXNjcmlwdGlvbj4gPC9yZGY6UkRGPiA8L3g6eG1wbWV0YT4gPD94cGFja2V0IGVuZD0iciI/PqXA0YIAAABnSURBVHjaYvj//z8DDAPBAiAGMRYgixPCuPQxQiXBgJGREc4BijMyEAnw6YPZSm28gBHKoAkYwoYjR+hosAwjw5FzFijHUcoH0UwMNAQ0NXw0zEfDfDTMKQTMQKwIxAY0MHshQIABAFt8pXTQ5lYVAAAAAElFTkSuQmCC",
            buttonImageOnly: true
        },
        errorMessages: {
            invalidDateFormatErrorText: "",
            valueIsSmallerThanMinimumText: "",
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

            var datepickerOptions = $.extend(true, {}, plugin.settings.datepickerOptions, {
                dateFormat: plugin.settings.dateDisplayFormat,
                onSelect: function () {
                    plugin.setDate(plugin.pickerElement.datepicker("getDate"));
                }
            });

            var minAttribute = plugin.pickerElement.attr("min");
            var maxAttribute = plugin.pickerElement.attr("max");
            if (typeof minAttribute !== "undefined" && minAttribute !== false) {
                datepickerOptions.minDate = moment(minAttribute).toDate();
            }
            if (typeof maxAttribute !== "undefined" && maxAttribute !== false) {
                datepickerOptions.maxDate = moment(maxAttribute).toDate();
            }

            plugin.pickerElement.datepicker(datepickerOptions);

            plugin.pickerElement.on("change", function () {
                plugin.setDate(plugin.pickerElement.val(), plugin.settings.dateDisplayFormat);
            });

            var storedDateMoment = plugin.getStoredValueMoment();
            if (storedDateMoment !== null) {
                plugin.setDate(storedDateMoment.toDate());
            }
        },

        setDate: function (value, dateFormat) {
            var plugin = this;
            
            var momentValue = null;
            if (value) {
                if (typeof value === "string") {
                    momentValue = moment(value, dateFormat);
                }
                else {
                    momentValue = moment(value);
                }
            }

            function showErrorAndRestore(message) {
                showErrorMessage(message);

                plugin.updateDisplayedValue();
            }

            if (momentValue === null || !momentValue.isValid()) {
                return showErrorAndRestore(plugin.settings.errorMessages.invalidDateFormatErrorText);
            }

            var date = momentValue.toDate();
            var minDate = plugin.pickerElement.datepicker("option", "minDate");
            var maxDate = plugin.pickerElement.datepicker("option", "maxDate");
            
            if (maxDate !== null && date > maxDate) {
                return showErrorAndRestore(plugin.settings.errorMessages.valueIsGreaterThanMaximumText);
            }

            if (minDate !== null && date < minDate) {
                return showErrorAndRestore(plugin.settings.errorMessages.valueIsSmallerThanMinimumText);
            }

            plugin.pickerElement.datepicker("setDate", date);

            plugin.updateStoredValue(date);

            plugin.settings.dateChanged(date);
        },

        getDate: function () {
            var plugin = this;

            var storeText = plugin.inputElement.val();

            return storeText.length > 0 ? plugin.pickerElement.datepicker("getDate") : null;
        },

        setOption: function (name, value) {
            var plugin = this;

            if (name && value) {
                plugin.pickerElement.datepicker("option", name, value);

                plugin.updateStoredValue();
            }
        },

        updateStoredValue: function (date) {
            var plugin = this;

            if (!date) {
                date = plugin.getDate();
            }

            if (date !== null) {
                plugin.inputElement.val(moment(date).format(plugin.settings.dateStoreFormat));
            }
            else {
                plugin.inputElement.val("");
            }
        },

        updateDisplayedValue: function (date) {
            var plugin = this;

            if (!date) {
                var storedValue = plugin.inputElement.val();
                date = storedValue ? moment(storedValue, plugin.settings.dateStoreFormat).toDate() : null;
            }

            if (date !== null) {
                plugin.pickerElement.datepicker("setDate", date);
            }
            else {
                plugin.pickerElement.val("");
            }
        },

        getStoredValueMoment: function () {
            var plugin = this;

            var storedValue = plugin.inputElement.val();
            return storedValue ? moment(storedValue, this.settings.dateStoreFormat) : null;
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