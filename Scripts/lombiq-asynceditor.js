/**
 * @summary     Lombiq - Async Editor
 * @description Manages async content editing with editor group support.
 * @version     1.0
 * @file        lombiq-asynceditor.js
 * @author      Lombiq Technologies Ltd.
 */

; (function ($, window, document, undefined) {
    "use strict";

    var pluginName = "lombiq_AsyncEditor";

    // Define static variables accessible using the plugin name as an object.
    var staticVariables = {
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
            parentEditorPostRequestedCallback: function (submitContext, eventContext) { },
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
        $editorContainerElement: null,
        $processingIndicatorElement: null,
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

            plugin.$editorContainerElement = plugin.element.find(plugin.settings.editorPlaceholderElementClass).first();
            plugin.$processingIndicatorElement = plugin.element.find(plugin.settings.processingIndicatorElementClass).first();

            // Find the closest potential parent AsyncEditor plugin.
            var $closestLoaderElement = plugin.element
                .parent()
                .closest(plugin.settings.asyncEditorLoaderElementClass);

            if ($closestLoaderElement.length > 0) {
                plugin.parentPlugin = $closestLoaderElement.lombiq_AsyncEditor()[0];
                plugin.parentPlugin.setChildPlugin(plugin);
            }

            // Load the editor group if the initial data was given.
            plugin.loadEditor(plugin.settings.initialContentItemId, plugin.settings.initialEditorGroupName);
        },

        /**
         * Loads the editor for the given content item ID and group async.
         * @param {number} contentItemId ID that identifies the content item that the editor is generated for.
         * @param {string} group Technical name of the editor group. Can be left empty.
         * @returns {Object} Returns the current plugin.
         */
        loadEditor: function (contentItemId, group) {
            this.getLoadEditorXHR(contentItemId, group);

            return this;
        },

        /**
         * Reloads the currently loaded editor.
         * @returns {Object} Returns the current plugin.
         */
        reloadEditor: function () {
            return this.loadEditor(plugin.currentContentItemId, plugin.currentGroup);
        },

        /**
         * Submits the currently loaded editor form. Child plugin will be alerted.
         * @param {Object} submitContext Contains information about the editor post (e.g. submit button details).
         * @returns {Object} Returns the current plugin.
         */
        postEditor: function (submitContext) {
            var plugin = this;

            var postEditorAjax = function () {
                plugin.getPostEditorXHR(submitContext);
            };
            if (!plugin.childPlugin) postEditorAjax();
            else {
                $.when(plugin.childPlugin.parentPostEditorRequested(submitContext, plugin)).done(postEditorAjax);
            }

            return plugin;
        },

        /**
         * Sets a plugin as a child plugin it can be alerted on specific events such as parent editor post.
         * @param {Object} childPlugin The child plugin object.
         * @returns {Object} Returns the current plugin.
         */
        setChildPlugin: function (childPlugin) {
            this.childPlugin = childPlugin;

            return this;
        },

        /**
         * Handles the parent post request.
         * @param {Object} submitContext Contains information about the editor post (e.g. submit button details).
         * @param {Object} parentPlugin Plugin object that has originally triggered the editor post.
         * @returns {Object} Deferred object that's result can be processed.
         */
        parentPostEditorRequested: function (submitContext, parentPlugin) {
            var plugin = this;

            var deferred = $.Deferred();

            var handle = function () {
                var eventContext = {
                    plugin: parentPlugin,
                    cancel: false,
                    postEditor: true
                };

                if (plugin.settings.callbacks.parentEditorPostRequestedCallback) {
                    plugin.settings.callbacks.parentEditorPostRequestedCallback(submitContext, eventContext);
                }

                if (eventContext.cancel) deferred.reject();
                else if (eventContext.postEditor) {
                    $.when(plugin.getPostEditorXHR(submitContext))
                        .done(function (response) {
                            if (response.Success && !response.HasValidationErrors) deferred.resolve();
                            else deferred.reject();
                        })
                        .fail(deferred.reject);
                }
            }

            if (plugin.childPlugin) {
                $.when(plugin.childPlugin.parentPostEditorRequested(submitContext, plugin))
                    .done(handle)
                    .fail(deferred.reject);
            }
            else {
                handle();
            }

            return deferred;
        },

        /**
         * Renders the given editor shape and also registers the necessary event listeners.
         * @param {string} editorShape HTML content of the editor shape.
         * @returns {Object} Returns the current plugin.
         */
        renderEditorShape: function (editorShape) {
            var plugin = this;

            if (!plugin.$editorContainerElement) return;

            if (!editorShape) {
                plugin.$editorContainerElement.hide();

                return;
            }

            plugin.childPlugin = null;

            plugin.$editorContainerElement.html($.parseHTML(editorShape, true));

            plugin.$editorContainerElement
                .find(plugin.settings.loadEditorActionElementClass)
                .on("click", function () {
                    var groupName = $(this).attr("data-editorGroupName");

                    if (groupName) plugin.loadEditor(plugin.currentContentItemId, groupName);
                });

            plugin.currentForm = plugin.$editorContainerElement
                .find("form")
                .first();

            if (plugin.currentForm) {
                plugin.currentForm.submit(function (e) {
                    e.preventDefault();
                });

                plugin.currentForm.find("input[type=submit]")
                    .click(function () {
                        var submitContext = plugin.createSubmitContext($(this));

                        plugin.postEditor(submitContext);
                    });
            }

            plugin.$editorContainerElement.show();

            return plugin;
        },

        /**
         * Evaluates the API response (e.g. sets the current group or content item ID and renders the editor shape).
         * @param {Object} response API response object.
         * @returns {Object} Returns the current plugin.
         */
        evaluateApiResponse: function (response) {
            var plugin = this;

            if (response.Success) {
                plugin.currentGroup = response.EditorGroup;
                plugin.currentContentItemId = response.ContentItemId;

                plugin.renderEditorShape(response.EditorShape);
            }

            if (response.ResultMessage) {
                alert(response.ResultMessage);
            }

            return plugin;
        },

        /**
         * Displays or hides the processing indicator.
         * @param {boolean} show Displays or hides the indicator depending on this value.
         * @returns {Object} Returns the current plugin.
         */
        setProcessingIndicatorVisibility: function (show) {
            var plugin = this;

            if (!plugin.$processingIndicatorElement) return;

            if (show) {
                plugin.$processingIndicatorElement.show();
            }
            else {
                plugin.$processingIndicatorElement.hide();
            }

            return plugin;
        },

        /**
         * Generates an async ajax request object for loading the editor.
         * @param {number} contentItemId ID that identifies the content item that the editor is generated for.
         * @param {string} group Technical name of the editor group. Can be left empty.
         * @returns {Object} XHR object for loading the editor async.
         */
        getLoadEditorXHR: function (contentItemId, group) {
            var plugin = this;

            return $.ajax({
                type: "GET",
                url: plugin.settings.asyncEditorApiUrl,
                data: {
                    contentItemId: contentItemId,
                    contentType: plugin.settings.contentType,
                    group: group
                },
                beforeSend: function () {
                    plugin.setProcessingIndicatorVisibility(true);
                },
                success: function (response) {
                    plugin.evaluateApiResponse(response);

                    if (plugin.settings.callbacks.editorLoadedCallback) {
                        plugin.settings.callbacks.editorLoadedCallback(response);
                    }
                },
                complete: function () {
                    plugin.setProcessingIndicatorVisibility(false);
                }
            });
        },

        /**
         * Generates an async ajax request object for posting the editor.
         * @param {Object} submitContext Contains information about the editor post (e.g. submit button details).
         * @returns {Object} XHR object for posting the editor async.
         */
        getPostEditorXHR: function (submitContext) {
            var plugin = this;
            
            return $.ajax({
                type: "POST",
                url: plugin.currentForm.attr("action"),
                data: plugin.currentForm.serialize() + (submitContext.submitButtonName && submitContext.submitButtonValue ?
                    ("&" + encodeURI(submitContext.submitButtonName) + "=" + encodeURI(submitContext.submitButtonValue)) : ""),
                beforeSend: function () {
                    plugin.setProcessingIndicatorVisibility(true);
                },
                success: function (response) {
                    plugin.evaluateApiResponse(response);

                    if (plugin.settings.callbacks.editorPostedCallback) {
                        plugin.settings.callbacks.editorPostedCallback(submitContext, response);
                    }
                },
                complete: function () {
                    plugin.setProcessingIndicatorVisibility(false);
                }
            });
        },

        /**
         * Generates a submit context object using the given submit button element.
         * @param {jQuery} $submitButtonElement jQuery object for the submit button element.
         * @returns {Object} The generated submit context.
         */
        createSubmitContext: function ($submitButtonElement) {
            return {
                name: $submitButtonElement.attr("data-submitContext"),
                submitButtonName: $submitButtonElement.attr("name"),
                submitButtonValue: $submitButtonElement.attr("value")
            };
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