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
        asyncEditorLoaderElementClass: "",
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
         * Initializes the Lombiq Async Editor plugin.
         */
        init: function () {
            var plugin = this;

            console.log("INIT FROM " + plugin.element.attr("id"));

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

        /**
         * Reloads the currently loaded editor.
         */
        reloadEditor: function () {
            this.loadEditor(plugin.currentContentItemId, plugin.currentGroup);
        },

        /**
         * Loads the editor for the given content item ID and group async.
         * @param {number} contentItemId ID that identifies the content item that the editor is generated for.
         * @param {string} group Technical name of the group. Can be left empty.
         */
        loadEditor: function (contentItemId, group) {
            var plugin = this;
            console.log("LOAD FROM " + plugin.element.attr("id") + " ID: " + contentItemId + " GROUP: " + group + " TYPE: " + plugin.settings.contentType);

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

                        plugin.currentGroup = response.EditorGroup;
                        plugin.currentContentItemId = response.ContentItemId;
                    }
                    else {
                        alert(response.ErrorMessage);
                    }

                    plugin.showProcessingIndicator(false);
                },
                error: function () {
                    plugin.showProcessingIndicator(false);
                }
            });
        },

        /**
         * Submits the currently loaded editor form.
         * @param {JQuery} submitButtonElement JQuery element for the submit button. Its name and value is also posted to the server.
         */
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
                error: function () {
                    plugin.showProcessingIndicator(false);
                }
            });
        },

        /**
         * Renders the given editor shape and also registers the necessary event listeners.
         * @param {string} editorShape HTML content of the editor shape.
         */
        renderEditorShape: function (editorShape) {
            var plugin = this;

            if (!plugin.editorContainerElement) return;

            if (!editorShape) {
                plugin.editorContainerElement.hide();

                return;
            }

            plugin.editorContainerElement.html($.parseHTML(editorShape, true));

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

        /**
         * Displays or hides the processing indicator.
         * @param {boolean} show Displays or hides the indicator depending on this value.
         */
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
        // "map" makes it possible to return the already existing or currently initialized plugin instances.
        return this.map(function () {
            if (!$.data(this, "plugin_" + pluginName)) {
                $.data(this, "plugin_" + pluginName, new Plugin($(this), options));
            }

            return $.data(this, "plugin_" + pluginName);
        });
    };
})(jQuery, window, document);