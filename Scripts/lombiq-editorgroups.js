﻿/**
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
        processingIndicatorElementClass: "",
        editorGroupName: "",
        editorPlaceholderElementClass: "",
        formElementClass: "",
        loadEditorActionElementClass: "",
        postEditorActionElementClass: "",
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
        processingIndicatorElement: null,
        currentGroup: "",
        currentContentItemId: 0,

        /**
         * Initializes the Lombiq EditorGroups plugin.
         */
        init: function () {
            var plugin = this;

            if (!plugin.settings.editorGroupName) return;

            plugin.editorContainerElement = plugin.element.find(plugin.settings.editorPlaceholderElementClass).first();
            plugin.processingIndicatorElement = plugin.element.find(plugin.settings.processingIndicatorElementClass).first();

            plugin.loadEditor(plugin.settings.contentItemId, plugin.settings.editorGroupName);
        },

        loadEditor: function (contentItemId, group) {
            var plugin = this;

            plugin.showProcessingIndicator(true);

            $.ajax({
                type: "GET",
                url: plugin.settings.asyncEditorApiUrl,
                data: {
                    contentItemId: contentItemId,
                    contentType: plugin.settings.contentType,
                    group: group
                },
                success: function (response) {
                    plugin.handleResponse(response)
                },
                fail: function () {
                    plugin.showProcessingIndicator(false);
                }
            });
        },

        handleResponse: function (response) {
            var plugin = this;
            console.log(response);
            if (response.Success) {
                plugin.showEditor(response.EditorShape);
            }
            else {
                alert(response.ErrorMessage);
            }

            plugin.currentGroup = response.EditorGroup;
            plugin.currentContentItemId = response.ContentItemId;

            plugin.showProcessingIndicator(false);
        },

        showProcessingIndicator: function (show) {
            var plugin = this;

            if (!plugin.processingIndicatorElement) return;

            if (show) {
                plugin.processingIndicatorElement.show();
            }
            else {
                plugin.processingIndicatorElement.hide();
            }
        },

        showEditor: function (content) {
            var plugin = this;

            if (!plugin.editorContainerElement) return;

            if (!content) {
                plugin.editorContainerElement.hide();

                return;
            }

            plugin.editorContainerElement.html(content);

            plugin.editorContainerElement
                .find(plugin.settings.loadEditorActionElementClass)
                .first()
                .on("click", function () {
                var groupName = $(this).attr("data-editorGroupName");

                if (groupName) plugin.loadEditor(plugin.currentContentItemId, groupName);
            });

            plugin.editorContainerElement
                .find(plugin.settings.postEditorActionElementClass)
                .first()
                .on("click", function () {
                console.log("posting...");
                plugin.showProcessingIndicator(true);

                var form = plugin.editorContainerElement.find("form");

                form.submit(function (e) {
                    e.preventDefault();
                });

                $.ajax({
                    type: "POST",
                    url: form.attr("action"),
                    data: form.serialize(),
                    success: function (response) {
                        plugin.handleResponse(response)
                    },
                    fail: function () {
                        plugin.showProcessingIndicator(false);
                    }
                });
            });

            plugin.editorContainerElement.show();
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