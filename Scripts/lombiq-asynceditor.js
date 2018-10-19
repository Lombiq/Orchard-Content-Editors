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
            editorPosting: pluginName + "_EditorPosting",
            editorPosted: pluginName + "_EditorPosted",
            parentEditorPostRequested: pluginName + "_ParentEditorPostRequested"
        },
        displayModes: {
            inline: "Inline",
            modal: "Modal"
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
        editorModalSettings: {
            dialogClass: "modalContainer",
            width: "800px",
            closeOnEscape: false,
            modal: true,
            autoOpen: true
        },
        callbacks: {
            parentEditorPostRequestedCallback: function (plugin, submitContext, eventContext) { },
            editorLoadedCallback: function (plugin, apiResponse) { },
            editorPostingCallback: function (plugin, context) { },
            editorPostedCallback: function (plugin, submitContext, apiResponse) { }
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
        currentDisplayMode: "",
        currentDisplayedEditorModal: null,

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

            return plugin.loadEditor(plugin.getContentItemId(), plugin.getEditorGroupName());
        },

        /**
         * Submits the currently loaded editor form. Child plugin will be alerted.
         * @param {Object} submitContext Contains information about the editor post (e.g. submit button details).
         * @returns {Object} Returns the current plugin.
         */
        postEditor: function (submitContext) {
            var plugin = this;

            var postingContext = {
                cancel: false
            };

            plugin.settings.callbacks.editorPostingCallback(plugin, submitContext, postingContext);
            $(plugin.element).trigger(staticVariables.eventNames.editorPosting, [plugin, submitContext, postingContext]);

            if (postingContext.cancel) return plugin;

            var postEditorAjax = function () {
                if (submitContext.formNoValidate || plugin.validateForm()) {
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
         * @returns {Boolean} True if the form is valid.
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
                    plugin.settings.callbacks.parentEditorPostRequestedCallback(plugin, currentSubmitContext, eventContext);
                }

                $(plugin.element).trigger(staticVariables.eventNames.parentEditorPostRequested, [plugin, currentSubmitContext, eventContext]);

                if (eventContext.cancel) deferred.reject();
                else {
                    var postingContext = {
                        cancel: false
                    };

                    plugin.settings.callbacks.editorPostingCallback(plugin, submitContext, postingContext);

                    $(plugin.element).trigger(staticVariables.eventNames.editorPosting, [plugin, submitContext, postingContext]);

                    if (postingContext.cancel) deferred.reject();
                    else if (eventContext.postEditor) {
                        if (submitContext.formNoValidate || plugin.validateForm()) {
                            $.when(plugin.getPostEditorXHR(currentSubmitContext))
                                .done(function (response) {
                                    if (response.Success && !response.HasValidationErrors) deferred.resolve();
                                    else deferred.reject();
                                })
                                .fail(deferred.reject);
                        }
                    }
                }
            };

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
         * Closes and destroys displayed editor modal if it was displayed.
         */
        destroyDisplayedEditorModal: function () {
            var plugin = this;

            // Make sure that there is a dialog already opened and not destroyed already (e.g. can happen if grandparent plugin
            // is being reloaded).
            if (plugin.currentDisplayedEditorModal &&
                $(plugin.element).children(".ui-dialog").children("[data-displayMode='Modal']").length > 0) {
                plugin.currentDisplayedEditorModal.dialog("destroy").remove();
                plugin.currentDisplayedEditorModal = null;
            }
        },

        /**
         * Renders the given editor shape and also registers the necessary event listeners.
         * @param {string} editorShape HTML content of the editor shape.
         * @param {string} displayMode Optional display mode for the editor shape. By default it's inline.
         * @param {string} appendEditor Indicates whether the editor shape needs to be appended to its container or replaced. By default it's false.
         * @returns {Object} Returns the current plugin.
         */
        renderEditorShape: function (editorShape, displayMode, appendEditor) {
            var plugin = this;

            var $editorContainer = plugin.$editorContainerElement;
            if (!$editorContainer) return;

            if (!editorShape) {
                $editorContainer.hide();

                return;
            }

            plugin.destroyDisplayedEditorModal();

            if (!displayMode) {
                // Try to determine display mode from the editor shape wrapper's attribute.
                var attributeDisplayMode = $(editorShape).attr("data-displayMode");

                plugin.currentDisplayMode = !attributeDisplayMode ?
                    staticVariables.displayModes.inline :
                    attributeDisplayMode;
            }

            if (appendEditor === undefined) {
                // Try to determine display mode from the editor shape wrapper's attribute.
                var attributeAppendEditor = $(editorShape).attr("data-appendEditor");

                appendEditor = attributeAppendEditor === "true" || attributeAppendEditor === "True";
            }

            if (plugin.childPlugin) {
                plugin.childPlugin.destroyDisplayedEditorModal();
                plugin.childPlugin = null;
            }
            
            var parsedEditorShape = $.parseHTML(editorShape, true);
            
            if (appendEditor) {
                $editorContainer.append(parsedEditorShape);
            }
            else {
                $editorContainer.html(parsedEditorShape);
            }

            $(parsedEditorShape)
                .find(plugin.settings.loadEditorActionElementClass)
                .on("click", function () {
                    var groupName = $(this).attr("data-editorGroupName");

                    if (plugin.confirmDirtyFormLeave()) {
                        if (plugin.currentGroup !== groupName) {
                            plugin.setGroupNameAndItemIdInUrl(groupName, plugin.getContentItemId());
                        }

                        if (groupName) plugin.loadEditor(plugin.currentContentItemId, groupName);
                    }
                });

            plugin.currentForm = $(parsedEditorShape)
                .find("form")
                .first();

            if (plugin.currentForm) {
                plugin.currentForm.submit(function (e) {
                    e.preventDefault();
                });

                plugin.currentForm.areYouSure({
                    message: plugin.settings.dirtyFormLeaveConfirmationText
                });

                plugin.currentForm.find("input[type=submit]")
                    .click(function () {
                        var submitContext = plugin.createSubmitContext($(this));

                        plugin.postEditor(submitContext);
                    });
            }

            if (plugin.currentDisplayMode === staticVariables.displayModes.modal) {
                if (!plugin.settings.editorModalSettings.appendTo) {
                    plugin.settings.editorModalSettings.appendTo = plugin.element;
                }
                plugin.currentDisplayedEditorModal = $editorContainer
                    .children()
                    .last()
                    .dialog(plugin.settings.editorModalSettings);
            }
            else {
                $editorContainer.show();
                $("html, body").scrollTop(0);
            }

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
                        plugin.settings.callbacks.editorLoadedCallback(plugin, response);
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
         * @returns {Plugin} Root plugin.
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
            if (submitContext.submitButtonName && submitContext.submitButtonValue) {
                formData.append(submitContext.submitButtonName, submitContext.submitButtonValue);
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
                    if (response.EditorGroup && plugin.currentGroup !== response.EditorGroup) {
                        plugin.setGroupNameAndItemIdInUrl(response.EditorGroup, response.ContentItemId);
                    }

                    plugin.evaluateApiResponse(response);

                    if (plugin.settings.callbacks.editorPostedCallback) {
                        plugin.settings.callbacks.editorPostedCallback(plugin, submitContext, response);
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
                submitButtonValue: $submitButtonElement.attr("value"),
                formNoValidate: $submitButtonElement.is("[formnovalidate]")
            };
        },

        /**
         * Helper for acquiring group name from query string.
         * @returns {String} Editor group name.
         */
        getEditorGroupNameFromUrl: function () {
            var plugin = this;

            return new URI().search(true)[plugin.groupNameQueryStringParameter];
        },

        /**
         * Helper for acquiring the group name.
         * @returns {String} Editor group name.
         */
        getEditorGroupName: function () {
            var plugin = this;

            return plugin.getEditorGroupNameFromUrl() || plugin.settings.defaultEditorGroupName;
        },

        /**
         * Helper for acquiring content item ID from query string.
         * @returns {String} Content item ID.
         */
        getContentItemIdFromUrl: function () {
            var plugin = this;

            return new URI().search(true)[plugin.contentItemIdQueryStringParameter];
        },

        /**
         * Helper for acquiring the content item ID.
         * @returns {String} Content item ID.
         */
        getContentItemId: function () {
            var plugin = this;

            return plugin.getContentItemIdFromUrl() || plugin.settings.initialContentItemId;
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
         * @returns {Array} List of query string parameters.
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
         * Checks if the form is dirty (ie.e inputs have been changed).
         * @returns {boolean} True if the form is dirty (i.e. inputs have been changed).
         */
        isEditorFormDirty: function () {
            return this.currentForm.hasClass("dirty");
        },

        /**
         * Checks if the form is dirty. If yes, displays a confirmation text.
         * @returns {Boolean} True if the form is not dirty or the user confirmed to leave dirty form.
         */
        confirmDirtyFormLeave: function () {
            var isDirty = this.isEditorFormDirty();

            if (!isDirty) return true;

            return confirm(this.settings.dirtyFormLeaveConfirmationText);
        }
    });

    $.fn[pluginName] = function (options) {
        // Return null if the element query is invalid.
        if (!this || this.length === 0) return null;

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