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
        },
        eventNames: {
            editorLoaded: pluginName + "_EditorLoaded",
            editorPosted: pluginName + "_EditorPosted",
            parentEditorPostRequested: pluginName + "_ParentEditorPostRequested"
        }
    };

    var defaults = {
        asyncEditorApiUrl: "",
        contentType: "",
        initialContentItemId: 0,
        defaultEditorGroupName: "",
        groupNameQueryStringParameter: "",
        contentItemIdQueryStringParameter: "",
        processingIndicatorElementClass: "",
        asyncEditorLoaderElementClass: "",
        editorPlaceholderElementClass: "",
        loadEditorActionElementClass: "",
        postEditorActionElementClass: "",
        dirtyFormLeaveConfirmationText: "Are you sure you want to leave this editor group? Changes you made may not be saved.",
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
        groupNameQueryStringParameter: "",
        contentItemIdQueryStringParameter: "",

        /**
         * Initializes the Lombiq Async Editor plugin.
         */
        init: function () {
            var plugin = this;

            plugin.$editorContainerElement = $(plugin.element).find(plugin.settings.editorPlaceholderElementClass).first();
            plugin.$processingIndicatorElement = $(plugin.element).find(plugin.settings.processingIndicatorElementClass).first();

            plugin.groupNameQueryStringParameter = plugin.settings.groupNameQueryStringParameter.length > 0 ?
                plugin.settings.groupNameQueryStringParameter :
                plugin.settings.contentType + "EditorGroup";

            plugin.contentItemIdQueryStringParameter = plugin.settings.contentItemIdQueryStringParameter.length > 0 ?
                plugin.settings.contentItemIdQueryStringParameter :
                plugin.settings.contentType + "Id";

            // Find the closest potential parent AsyncEditor plugin.
            var $closestLoaderElement = $(plugin.element)
                .parent()
                .closest(plugin.settings.asyncEditorLoaderElementClass);

            if ($closestLoaderElement.length > 0) {
                plugin.parentPlugin = $closestLoaderElement.lombiq_AsyncEditor()[0];
                plugin.parentPlugin.setChildPlugin(plugin);
            }

            // Load the editor group if the initial data was given.
            var getEditorGroup = function () {
                return plugin.getEditorGroupNameFromUrl() ||
                    plugin.settings.defaultEditorGroupName;
            }

            plugin.reloadEditor();

            window.onpopstate = function (e) {
                plugin.getRootPlugin().reloadEditor();
            };
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
            var plugin = this;

            var contentItemId = plugin.getContentItemIdFromUrl() ||
                plugin.settings.initialContentItemId;

            var editorGroup = plugin.getEditorGroupNameFromUrl() ||
                plugin.settings.defaultEditorGroupName;

            return plugin.loadEditor(contentItemId || plugin.settings.initialContentItemId, editorGroup);
        },

        /**
         * Submits the currently loaded editor form. Child plugin will be alerted.
         * @param {Object} submitContext Contains information about the editor post (e.g. submit button details).
         * @returns {Object} Returns the current plugin.
         */
        postEditor: function (submitContext) {
            var plugin = this;

            var postEditorAjax = function () {
                if (plugin.validateForm()) {
                    plugin.getPostEditorXHR(submitContext);
                }
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
         * Triggers a form validation and returns the form validity.
         * @returns True if the form is valid.
         */
        validateForm: function () {
            return this.currentForm[0].checkValidity();
        },

        /**
         * Handles the parent post request.
         * @param {Object} submitContext Contains information about the editor post (e.g. submit button details).
         * @param {Object} parentPlugin Plugin object that has originally triggered the editor post.
         * @returns {Object} Deferred object that's result can be processed.
         */
        parentPostEditorRequested: function (submitContext, parentPlugin) {
            var plugin = this;
            var currentSubmitContext = {};
            $.extend(currentSubmitContext, submitContext);

            var deferred = $.Deferred();

            var handle = function () {
                var eventContext = {
                    plugin: parentPlugin,
                    cancel: false,
                    postEditor: true
                };

                if (plugin.settings.callbacks.parentEditorPostRequestedCallback) {
                    plugin.settings.callbacks.parentEditorPostRequestedCallback(currentSubmitContext, eventContext);
                }

                $(plugin.element).trigger(staticVariables.eventNames.parentEditorPostRequested, [plugin, currentSubmitContext, eventContext]);
                
                if (eventContext.cancel) deferred.reject();
                else if (eventContext.postEditor) {
                    if (plugin.validateForm()) {
                        $.when(plugin.getPostEditorXHR(currentSubmitContext))
                            .done(function (response) {
                                if (response.Success && !response.HasValidationErrors) deferred.resolve();
                                else deferred.reject();
                            })
                            .fail(deferred.reject);
                    }
                }
            }

            if (plugin.childPlugin) {
                // Alert children with the original submit context.
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

                    if (plugin.confirmDirtyFormLeave()) {
                        if (plugin.currentGroup != groupName) {
                            plugin.setGroupNameAndItemIdInUrl(groupName, plugin.getContentItemIdFromUrl() || plugin.currentContentItemId)
                        }

                        if (groupName) plugin.loadEditor(plugin.currentContentItemId, groupName);
                    }
                });

            plugin.currentForm = plugin.$editorContainerElement
                .find("form")
                .first();

            if (plugin.currentForm) {
                plugin.currentForm.submit(function (e) {
                    e.preventDefault();
                });

                plugin.currentForm.areYouSure({
                    message: plugin.settings.dirtyFormLeaveConfirmationText,
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

            // Use static loading indicator if processing indicator element is not given.
            var useStaticLoadingIndicator = !plugin.settings.processingIndicatorElementClass;

            if (show) {
                if (useStaticLoadingIndicator) $.lombiq_LoadingIndicator.show();
                else plugin.$processingIndicatorElement.show();
            }
            else {
                if (useStaticLoadingIndicator) $.lombiq_LoadingIndicator.hide();
                else plugin.$processingIndicatorElement.hide();
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

                    $(plugin.element).trigger(staticVariables.eventNames.editorLoaded, [plugin, response]);
                },
                complete: function () {
                    plugin.setProcessingIndicatorVisibility(false);
                }
            });
        },

        /**
         * Returns the root plugin from the hierarchy.
         * @returns Root plugin.
         */
        getRootPlugin: function () {
            var plugin = this;

            while (plugin) {
                if (plugin.parentPlugin) plugin = plugin.parentPlugin;
                else return plugin;
            }
        },

        /**
         * Generates an async ajax request object for posting the editor.
         * @param {Object} submitContext Contains information about the editor post (e.g. submit button details).
         * @returns {Object} XHR object for posting the editor async.
         */
        getPostEditorXHR: function (submitContext) {
            var plugin = this;
            // For file uploads to work FormData needs to be used, and processData, contentType needs to be false.
            var formData = new FormData(plugin.currentForm[0]);
            if (!submitContext.submitButtonName || !submitContext.submitButtonValue) {
                formData.delete(submitContext.submitButtonName);
            }

            return $.ajax({
                type: "POST",
                url: plugin.currentForm.attr("action"),
                data: formData,
                processData: false,
                contentType: false,
                beforeSend: function () {
                    plugin.setProcessingIndicatorVisibility(true);
                },
                success: function (response) {
                    if (response.EditorGroup && plugin.currentGroup != response.EditorGroup) {
                        plugin.setGroupNameAndItemIdInUrl(response.EditorGroup, response.ContentItemId);
                    }

                    plugin.evaluateApiResponse(response);

                    if (plugin.settings.callbacks.editorPostedCallback) {
                        plugin.settings.callbacks.editorPostedCallback(submitContext, response);
                    }

                    $(plugin.element).trigger(staticVariables.eventNames.editorPosted, [plugin, submitContext, response]);
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

        /**
         * Helper for acquiring content item ID from query string.
         * @returns Content item ID.
         */
        getEditorGroupNameFromUrl: function () {
            var plugin = this;

            return new URI().search(true)[plugin.groupNameQueryStringParameter];
        },

        /**
         * Helper for acquiring group name from query string.
         * @returns Editor group name.
         */
        getContentItemIdFromUrl: function () {
            var plugin = this;

            return new URI().search(true)[plugin.contentItemIdQueryStringParameter];
        },

        /**
         * Helper for updating group name and content item ID query string parameter.
         * @param {string} groupName Name of the editor group.
         * @param {number} contentItemId ID of the content item.
         */
        setGroupNameAndItemIdInUrl: function (groupName, contentItemId) {
            var plugin = this;

            var uri = new URI();
            var parametersToRemove = plugin.getQueryStringParameterNames(true);

            uri.removeSearch(parametersToRemove);

            if (groupName && groupName.length > 0) {
                uri.setSearch(plugin.groupNameQueryStringParameter, groupName);
            }

            if (contentItemId) {
                uri.setSearch(plugin.contentItemIdQueryStringParameter, contentItemId);
            }

            history.pushState(groupName, "", uri.pathname() + uri.search());
        },

        /**
         * Returns a list of query string parameters used by this async editor plugin. 
         * Optionally includes parameters used by child plugins as well.
         * @param {boolean} deep Include query string parameters used by child plugins.
         * @returns List of query string parameters.
         */
        getQueryStringParameterNames: function (deep) {
            var plugin = this;
            var parameters = [
                plugin.groupNameQueryStringParameter,
                plugin.contentItemIdQueryStringParameter
            ];

            if (deep && plugin.childPlugin) {
                $.merge(parameters, plugin.childPlugin.getQueryStringParameterNames(true));
            }

            return parameters;
        },

        /**
         * Checks if the form is dirty. If yes, displays a confirmation text.
         * @returns True if the form is not dirty or the user confirmed to leave dirty form.
         */
        confirmDirtyFormLeave: function () {
            var isDirty = this.currentForm.hasClass("dirty");

            if (!isDirty) return true;

            return confirm(this.settings.dirtyFormLeaveConfirmationText);
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

    $[pluginName] = staticVariables;
})(jQuery, window, document);