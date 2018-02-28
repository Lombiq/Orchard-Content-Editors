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

    // Define static variables accessible using the plugin name as an object.
    $[pluginName] = {
        submitContextNames: {
            Save: "Save",
            SaveAndNext: "SaveAndNext",
            Publish: "Publish"
        }
    };

    var defaults = {
        asyncEditorApiUrl: "",
        contentType: "",
        initialContentItemId: 0,
        initialEditorGroupName: "",
        processingIndicatorElementClass: "",
        asyncEditorLoaderElementClass: "",
        editorPlaceholderElementClass: "",
        loadEditorActionElementClass: "",
        postEditorActionElementClass: "",
        callbacks: {
            parentPostRequestedCallback: function (submitContext, eventContext) { },
            editorLoadedCallback: function (apiResponse) { },
            editorPostedCallback: function (submitContext, apiResponse) { }
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
        editorContainerElement: null,
        processingIndicatorElement: null,
        currentGroup: "",
        currentContentItemId: 0,
        currentForm: null,
        parentPlugin: null,
        childPlugin: null,

        /**
         * Initializes the Lombiq Async Editor plugin.
         */
        init: function () {
            var plugin = this;

            plugin.events.plugin = this;

            plugin.editorContainerElement = plugin.element.find(plugin.settings.editorPlaceholderElementClass).first();
            plugin.processingIndicatorElement = plugin.element.find(plugin.settings.processingIndicatorElementClass).first();
            
            var closestLoaderElement = plugin.element
                .parent()
                .closest(plugin.settings.asyncEditorLoaderElementClass);

            if (closestLoaderElement.length > 0) {
                plugin.parentPlugin = closestLoaderElement.lombiq_AsyncEditor()[0];
                plugin.parentPlugin.setChildPlugin(plugin);
            }

            plugin.loadEditor(plugin.settings.initialContentItemId, plugin.settings.initialEditorGroupName);
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

            plugin.setProcessingIndicatorVisibility(true);

            $.ajax({
                type: "GET",
                url: plugin.settings.asyncEditorApiUrl,
                data: {
                    contentItemId: contentItemId,
                    contentType: plugin.settings.contentType,
                    group: group
                },
                success: function (response) {
                    plugin.evaluateApiResponse(response);

                    plugin.events.editorLoaded(response);
                },
                error: function () {
                    plugin.setProcessingIndicatorVisibility(false);
                }
            });
        },

        createSubmitContext: function (submitButtonElement) {
            return {
                name: submitButtonElement.attr("data-submitContext"),
                submitButtonName: submitButtonElement.attr("name"),
                submitButtonValue: submitButtonElement.attr("value")
            };
        },

        /**
         * Submits the currently loaded editor form.
         * @param {JQuery} submitButtonElement JQuery element for the submit button. Its name and value is also posted to the server.
         */
        postEditor: function (submitContext, callback) {
            var plugin = this;

            plugin.setProcessingIndicatorVisibility(true);

            $.ajax({
                type: "POST",
                url: plugin.currentForm.attr("action"),
                data: plugin.currentForm.serialize() + (submitContext.submitButtonName && submitContext.submitButtonValue ?
                    ("&" + encodeURI(submitContext.submitButtonName) + "=" + encodeURI(submitContext.submitButtonValue)) : ""),
                success: function (response) {
                    plugin.evaluateApiResponse(response);

                    plugin.events.editorPosted(submitContext, response);

                    if (callback) callback(submitContext, response);
                },
                error: function () {
                    plugin.setProcessingIndicatorVisibility(false);
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

            plugin.childPlugin = null;

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
                        var submitContext = plugin.createSubmitContext($(this));

                        plugin.events.parentPostRequested(submitContext, plugin, function (submitContext, success) {
                            if (success) plugin.postEditor(submitContext);
                        });
                    });
            }

            plugin.editorContainerElement.show();
        },

        evaluateApiResponse: function (response) {
            var plugin = this;

            if (response.Success) {
                plugin.renderEditorShape(response.EditorShape);

                plugin.currentGroup = response.EditorGroup;
                plugin.currentContentItemId = response.ContentItemId;
            }

            if (response.ResultMessage) {
                alert(response.ResultMessage);
            }

            plugin.setProcessingIndicatorVisibility(false);
        },

        /**
         * Displays or hides the processing indicator.
         * @param {boolean} show Displays or hides the indicator depending on this value.
         */
        setProcessingIndicatorVisibility: function (show) {
            var plugin = this;

            if (!plugin.processingIndicatorElement) return;

            if (show) {
                plugin.processingIndicatorElement.show();
            }
            else {
                plugin.processingIndicatorElement.hide();
            }
        },

        setChildPlugin: function (childPlugin) {
            this.childPlugin = childPlugin;
        },

        events: {
            plugin: null,

            parentPostRequested: function (submitContext, parentPlugin, callback) {
                var plugin = this.plugin;

                if (!parentPlugin || !callback) return;

                var handleParentPostRequested = function () {
                    var eventContext = {
                        plugin: parentPlugin,
                        cancel: false,
                        postAll: true
                    };

                    if (plugin.settings.callbacks.parentPostRequestedCallback) {
                        plugin.settings.callbacks.parentPostRequestedCallback(submitContext, eventContext);
                    }

                    if (eventContext.cancel) return callback(false);

                    if (eventContext.postAll) {
                        plugin.postEditor(submitContext, function (submitContext, response) {
                            return callback(submitContext, response.Success && !response.HasValidationErrors);
                        })
                    }
                }

                if (!plugin.childPlugin) return handleParentPostRequested();

                plugin.childPlugin.events.parentPostRequested(submitContext, parentPlugin, function (success) {
                    if (!success) return callback(false);

                    handleParentPostRequested();
                });
            },

            editorLoaded: function (apiResponse) {
                var plugin = this.plugin;

                if (plugin.settings.callbacks.editorLoadedCallback) {
                    plugin.settings.callbacks.editorLoadedCallback(apiResponse);
                }
            },

            editorPosted: function (submitContext, apiResponse) {
                var plugin = this.plugin;

                if (plugin.settings.callbacks.editorPostedCallback) {
                    plugin.settings.callbacks.editorPostedCallback(submitContext, apiResponse);
                }
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