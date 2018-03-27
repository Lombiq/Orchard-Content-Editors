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
        multipleEditorsNotAllowedMessage: "Editing multiple items at the same is not allowed. Please save or cancel your current changes first.",
        onSaveSuccess: function () { },
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
        $addNewItemActionElement: null,
        $editItemActionElement: null,
        $editorPlaceholderElement: null,

        init: function () {
            var plugin = this;

            plugin.$editorPlaceholderElement = plugin.element.find(plugin.settings.editorPlaceholderElementClass);

            plugin.$addNewItemActionElement = plugin.element.find(plugin.settings.addNewItemActionElementClass).first()
                .on("click", function () {
                    plugin.getItemEditor(0, $editorPlaceholderElement);
                });

            plugin.$editItemActionElement = plugin.element.find(plugin.settings.editItemActionElementClass);
        },

        getItemEditor: function (url, $editorPlaceholder) {
            var plugin = this;

            if (plugin.concurrentEditors > 0) {
                alert(plugin.settings.multipleEditorsNotAllowedMessage);

                return;
            }
            
            $.ajax({
                url: url,
                type: "GET"
            }).success(function (data) {
                $editorPlaceholder.html(data);
                plugin.concurrentEditors++;
                
                //$editorPlaceholder.find(plugin.settings.cancelButtonElementClass).on("click", function () {
                //    $editorPlaceholder.html("");

                //    plugin.concurrentEditors--;
                //});

                $("html, body").animate({
                    scrollTop: $editorPlaceholder.offset().top
                }, 500);
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

    $[pluginName] = staticVariables;
})(jQuery, window, document);