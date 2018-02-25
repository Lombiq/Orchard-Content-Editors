/**
 * @summary     Lombiq - Async Editor
 * @description Manages async content editing with content editor support.
 * @version     1.0
 * @file        lombiq-asynceditor.js
 * @author      Lombiq Technologies Ltd.
 */

; (function ($, window, document, undefined) {
    "use strict";

    var pluginName = "lombiq_AsyncEditor";

    var defaults = {
        asyncEditorApiUrl: "",
        contentType: "",
        contentItemId: 0,
        processingIndicatorElementClass: "",
        editorGroupName: "",
        editorPlaceholderElementClass: "",
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
        currentForm: null,

        /**
         * Initializes the Lombiq EditorGroups plugin.
         */
        init: function () {
            var plugin = this;

            plugin.editorContainerElement = plugin.element.find(plugin.settings.editorPlaceholderElementClass).first();
            plugin.processingIndicatorElement = plugin.element.find(plugin.settings.processingIndicatorElementClass).first();

            var asyncEditorCallback = function (response) {
                console.log(response);

                if (response.Success) {
                    plugin.renderEditorShape(response.editorShape);
                }
                else {
                    alert(response.ErrorMessage);
                }

                plugin.currentGroup = response.EditorGroup;
                plugin.currentContentItemId = response.ContentItemId;

                plugin.showProcessingIndicator(false);
            };


            plugin.loadEditor(plugin.settings.contentItemId, plugin.settings.editorGroupName, asyncEditorCallback);
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
                    if (response.Success) {
                        plugin.renderEditorShape(response.EditorShape);
                    }
                    else {
                        alert(response.ErrorMessage);
                    }

                    plugin.currentGroup = response.EditorGroup;
                    plugin.currentContentItemId = response.ContentItemId;

                    plugin.showProcessingIndicator(false);
                },
                fail: function () {
                    plugin.showProcessingIndicator(false);
                }
            });
        },

        postEditor: function (submitButtonElement) {
            var plugin = this;

            plugin.showProcessingIndicator(true);

            $.ajax({
                type: "POST",
                url: plugin.currentForm.attr("action"),
                data: plugin.currentForm.serialize() + (submitButtonElement ?
                    ("&" + encodeURI(submitButtonElement.attr("name")) + "=" + encodeURI(submitButtonElement.attr("value"))) : ""),
                success: function (response) {
                    if (response.Success) {
                        plugin.renderEditorShape(response.EditorShape);
                    }
                    else {
                        alert(response.ErrorMessage);
                    }

                    plugin.currentGroup = response.EditorGroup;
                    plugin.currentContentItemId = response.ContentItemId;

                    plugin.showProcessingIndicator(false);
                },
                fail: function () {
                    plugin.showProcessingIndicator(false);
                }
            });
        },

        renderEditorShape: function (editorShape) {
            var plugin = this;

            if (!plugin.editorContainerElement) return;

            if (!editorShape) {
                plugin.editorContainerElement.hide();

                return;
            }

            plugin.editorContainerElement.html(editorShape);

            plugin.editorContainerElement
                .find(plugin.settings.loadEditorActionElementClass)
                .on("click", function () {
                    var groupName = $(this).attr("data-editorGroupName");

                    if (groupName) plugin.loadEditor(plugin.currentContentItemId, groupName);
                });

            plugin.currentForm = plugin.editorContainerElement
                .find("form")
                .first();

            if (plugin.currentForm) {
                plugin.currentForm.submit(function (e) {
                    e.preventDefault();
                });

                plugin.currentForm.find("input[type=submit]")
                    .click(function () {
                        plugin.postEditor($(this));
                    });
            }

            plugin.editorContainerElement.show();
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