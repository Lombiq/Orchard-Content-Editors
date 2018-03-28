/**
 * @summary     Lombiq - Async Editor Wrapper
 * @description Manages multiple async editors (i.e. async editors for content item lists).
 * @version     1.0
 * @file        lombiq-asynceditorwrapper.js
 * @author      Lombiq Technologies Ltd.
 */

; (function ($, window, document, undefined) {
    "use strict";

    var pluginName = "lombiq_AsyncEditorWrapper";

    var defaults = {
        editorPlaceholderElementClass: "",
        addNewItemActionElementClass: "",
        editItemActionElementClass: "",
        cancelButtonElementClass: "",
        allowMultipleEditors: false,
        multipleEditorsNotAllowedMessage: "Editing multiple items at the same is not allowed. Please save or cancel your current changes first.",
        editorLoadedCallback: function (data, $editor) { },
    };
    
    function Plugin(element, options) {
        this.element = element;
        this.settings = $.extend(true, {}, defaults, options);
        this._defaults = defaults;
        this._name = pluginName;

        this.init();
    }

    $.extend(Plugin.prototype, {
        concurrentEditors: 0,

        init: function () {
            var plugin = this;

            var $editorPlaceholderElement = plugin.element.find(plugin.settings.editorPlaceholderElementClass);

            if (plugin.settings.addNewItemActionElementClass) {
                plugin.element.find(plugin.settings.addNewItemActionElementClass).first().on("click", function () {
                    plugin.loadEditor($(this).attr("data-url"), $editorPlaceholderElement);
                });
            }

            if (plugin.settings.editItemActionElementClass) {
                plugin.element.find(plugin.settings.editItemActionElementClass).on("click", function () {
                    plugin.loadEditor($(this).attr("data-url"), plugin.$editorPlaceholderElement);
                });
            }
        },

        loadEditor: function (url, $editorPlaceholder) {
            var plugin = this;

            if (!plugin.allowMultipleEditors && plugin.concurrentEditors > 0) {
                alert(plugin.settings.multipleEditorsNotAllowedMessage);

                return;
            }
            
            $.ajax({
                url: url,
                type: "GET"
            }).success(function (data) {
                if (data.Success) {
                    $editorPlaceholder.html($.parseHTML(data.EditorShape, true)).show();
                    plugin.concurrentEditors++;

                    if (plugin.settings.cancelButtonElementClass) {
                        $editorPlaceholder.find(plugin.settings.cancelButtonElementClass).on("click", function () {
                            $editorPlaceholder.html("").hide();

                            plugin.concurrentEditors--;
                        });
                    }

                    $("html, body").animate({
                        scrollTop: $editorPlaceholder.offset().top
                    }, 500);
                    
                    plugin.settings.editorLoadedCallback.call(plugin, data, $editorPlaceholder);
                }

                if (data.ResultMessage) {
                    alert(data.ResultMessage);
                }
            });
        },
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
})(jQuery, window, document);