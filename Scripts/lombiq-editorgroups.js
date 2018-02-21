/**
 * @summary     Lombiq - Editor Groups
 * @description Manages async content editing using editor groups.
 * @version     1.0
 * @file        lombiq-editorgroups.js
 * @author      Lombiq Technologies Ltd.
 */

; (function ($, window, document, undefined) {
    "use strict";

    var pluginName = "lombiq_EditorGroups";

    var defaults = {
        asyncEditorApiUrl: "",
        contentType: "",
        contentItemId: 0,
        processingIndicatorContent: "",
        editorGroupName: "",
        editorGroupLinkElementClass: ""
    };

    function Plugin(element, options) {
        this.element = element;
        this.settings = $.extend(true, {}, defaults, options);
        this._defaults = defaults;
        this._name = pluginName;

        this.init();
    }

    $.extend(Plugin.prototype, {
        editorContainerElement: null,
        
        /**
         * Initializes the Lombiq EditorGroups plugin.
         */
        init: function () {
            var plugin = this;

            if (!plugin.settings.editorGroupName) return;

            plugin.editorContainerElement = plugin.element.find(plugin.settings.editorContainerCssClassName);

            plugin.loadEditor();
        },

        loadEditor: function (group) {
            var plugin = this;

            if (!group) {
                group = plugin.settings.editorGroupName
            }

            plugin.element.empty();
            plugin.element.html(plugin.settings.processingIndicatorContent);

            $.ajax({
                type: "GET",
                url: plugin.settings.asyncEditorApiUrl,
                data: {
                    contentItemId: plugin.settings.contentItemId,
                    contentType: plugin.settings.contentType,
                    group: group
                },
                success: function (response) {
                    if (response.Success) {
                        plugin.element.empty();
                        console.log("succes - ", response);
                        plugin.element.html(response.EditorShape);
                    }
                    else {
                        alert(response.ErrorMessage);
                    }
                }
            });
        }
    });

    $.fn[pluginName] = function (options) {
        var plugin = new Plugin(this, options);

        if (!$.data(this, "plugin_" + pluginName)) {
            $.data(this, "plugin_" + pluginName, plugin);
        }

        return plugin;
    };
})(jQuery, window, document);