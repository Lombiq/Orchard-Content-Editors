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
        dateTimeValueInputClassName: "",
        invalidDateFormatErrorText: "",
        dateDisplayFormat: "MM/DD/YYYY",
        dateStoreFormat: "YYYY-MM-DD",
        dateChanged: function (value) { },
        datepickerOptions: {
            showButtonPanel: true,
            showOn: "button",
            buttonImage: "data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAABcAAAAYCAYAAAARfGZ1AAAAGXRFWHRTb2Z0d2FyZQBBZG9iZSBJbWFnZVJlYWR5ccllPAAAA+5pVFh0WE1MOmNvbS5hZG9iZS54bXAAAAAAADw/eHBhY2tldCBiZWdpbj0i77u/IiBpZD0iVzVNME1wQ2VoaUh6cmVTek5UY3prYzlkIj8+IDx4OnhtcG1ldGEgeG1sbnM6eD0iYWRvYmU6bnM6bWV0YS8iIHg6eG1wdGs9IkFkb2JlIFhNUCBDb3JlIDUuMy1jMDExIDY2LjE0NTY2MSwgMjAxMi8wMi8wNi0xNDo1NjoyNyAgICAgICAgIj4gPHJkZjpSREYgeG1sbnM6cmRmPSJodHRwOi8vd3d3LnczLm9yZy8xOTk5LzAyLzIyLXJkZi1zeW50YXgtbnMjIj4gPHJkZjpEZXNjcmlwdGlvbiByZGY6YWJvdXQ9IiIgeG1sbnM6eG1wTU09Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC9tbS8iIHhtbG5zOnN0UmVmPSJodHRwOi8vbnMuYWRvYmUuY29tL3hhcC8xLjAvc1R5cGUvUmVzb3VyY2VSZWYjIiB4bWxuczp4bXA9Imh0dHA6Ly9ucy5hZG9iZS5jb20veGFwLzEuMC8iIHhtbG5zOmRjPSJodHRwOi8vcHVybC5vcmcvZGMvZWxlbWVudHMvMS4xLyIgeG1wTU06T3JpZ2luYWxEb2N1bWVudElEPSJ1dWlkOjY1RTYzOTA2ODZDRjExREJBNkUyRDg4N0NFQUNCNDA3IiB4bXBNTTpEb2N1bWVudElEPSJ4bXAuZGlkOjg0M0YyRDVDMDY3QjExRTI5OUZEQTZGODg4RDc1ODdCIiB4bXBNTTpJbnN0YW5jZUlEPSJ4bXAuaWlkOjg0M0YyRDVCMDY3QjExRTI5OUZEQTZGODg4RDc1ODdCIiB4bXA6Q3JlYXRvclRvb2w9IkFkb2JlIFBob3Rvc2hvcCBDUzYgKE1hY2ludG9zaCkiPiA8eG1wTU06RGVyaXZlZEZyb20gc3RSZWY6aW5zdGFuY2VJRD0ieG1wLmlpZDowMTgwMTE3NDA3MjA2ODExODA4M0ZFMkJBM0M1RUU2NSIgc3RSZWY6ZG9jdW1lbnRJRD0ieG1wLmRpZDowNjgwMTE3NDA3MjA2ODExODA4M0U3NkRBMDNEMDVDMSIvPiA8ZGM6dGl0bGU+IDxyZGY6QWx0PiA8cmRmOmxpIHhtbDpsYW5nPSJ4LWRlZmF1bHQiPmdseXBoaWNvbnM8L3JkZjpsaT4gPC9yZGY6QWx0PiA8L2RjOnRpdGxlPiA8L3JkZjpEZXNjcmlwdGlvbj4gPC9yZGY6UkRGPiA8L3g6eG1wbWV0YT4gPD94cGFja2V0IGVuZD0iciI/PqXA0YIAAABnSURBVHjaYvj//z8DDAPBAiAGMRYgixPCuPQxQiXBgJGREc4BijMyEAnw6YPZSm28gBHKoAkYwoYjR+hosAwjw5FzFijHUcoH0UwMNAQ0NXw0zEfDfDTMKQTMQKwIxAY0MHshQIABAFt8pXTQ5lYVAAAAAElFTkSuQmCC",
            buttonImageOnly: true,
        }
    };

    function Plugin(element, options) {
        this.element = element;
        this.settings = $.extend(true, {}, defaults, options);
        this._defaults = defaults;
        this._name = pluginName;

        this.init();
    }

    $.extend(Plugin.prototype, {
        $datepicker: null,
        $dateTimeValueInput: null,

        init: function () {
            var plugin = this;

            plugin.$datepicker = $(plugin.element);
            plugin.$dateTimeValueInput = plugin.$datepicker.siblings(plugin.settings.dateTimeValueInputClassName).first();

            var datepickerOptions = $.extend(true, {}, plugin.settings.datepickerOptions, {
                dateFormat: plugin.settings.dateDisplayFormat,
                onSelect: function () {
                    plugin.setDate($(this).datepicker("getDate"));
                }
            });

            plugin.$datepicker.datepicker(datepickerOptions);

            plugin.$datepicker.on("change", function () {
                plugin.setDate(plugin.$datepicker.val());
            });
        },

        setDate: function (value, dateFormat) {
            var plugin = this;

            var displayText = "";
            var storeText = "";
            if (value) {
                var momentValue = null;

                if (typeof value === "string") {
                    var format = plugin.settings.dateDisplayFormat;
                    if (dateFormat) format = dateFormat;

                    momentValue = moment(value, format);
                }
                else {
                    momentValue = moment(value);
                }

                if (momentValue.isValid()) {
                    displayText = momentValue.format(plugin.settings.dateDisplayFormat);
                    storeText = momentValue.format(plugin.settings.dateStoreFormat);
                }
                else {
                    if (plugin.settings.invalidDateFormatErrorText.length > 0) {
                        alert(plugin.settings.invalidDateFormatErrorText);
                    }
                }
            }

            plugin.$datepicker.val(displayText);
            plugin.$dateTimeValueInput.val(storeText);

            plugin.settings.dateChanged(plugin.getDate());
        },

        getDate: function () {
            var plugin = this;

            var storeText = plugin.$dateTimeValueInput.val();

            return storeText.length > 0 ? moment(storeText).toDate() : null;
        },

        setOption: function (name, value) {
            if (name && value) {
                this.$datepicker.datepicker("option", name, value);
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
                $.data(this, "plugin_" + pluginName, new Plugin(this, options));
            }

            // ... and then return the plugin instance, which might be null
            // if the plugin is not instantiated on this element and "options" is undefined.
            return $.data(this, "plugin_" + pluginName);
        });
    };
})(jQuery, window, document);