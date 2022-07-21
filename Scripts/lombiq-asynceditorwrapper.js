﻿/**
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
        editUrl: "",
        deleteUrl: "",
        requestToken: "",
        editorPlaceholderElementClass: "",
        addNewItemActionElementClass: "",
        editItemActionElementClass: "",
        deleteItemActionElementClass: "",
        cancelButtonElementClass: "",
        allowMultipleEditors: false,
        contentItemIdQueryStringParameter: "",
        multipleEditorsNotAllowedMessage: "Editing multiple items at the same is not allowed. Please save or cancel your current changes first.",
        deleteItemConfirmationText: "Are you sure you want to delete this item?",
        editorLoadedCallback: function (plugin, data, $editor) { },
        deleteCallback: function (data) { },
        cancelCallback: function ($editor) { }
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

            var $editorPlaceholder = plugin.element.find(plugin.settings.editorPlaceholderElementClass);

            if (plugin.settings.addNewItemActionElementClass) {
                $(document).on("click", plugin.settings.addNewItemActionElementClass, function () {
                    plugin.loadEditor($(this).attr("data-contentItemId"), $editorPlaceholder);
                });
            }

            if (plugin.settings.editItemActionElementClass) {
                $(document).on("click", plugin.settings.editItemActionElementClass, function () {
                    plugin.loadEditor($(this).attr("data-contentItemId"), $editorPlaceholder);
                });
            }

            if (plugin.settings.deleteItemActionElementClass) {
                $(plugin.element).on("click", plugin.settings.deleteItemActionElementClass, function () {
                    var contentItemId = $(this).attr("data-contentItemId");
                    if (plugin.settings.deleteItemConfirmationText) {
                        if (confirm(plugin.settings.deleteItemConfirmationText)) {
                            plugin.deleteContentItem(contentItemId);
                        }
                    }
                    else {
                        plugin.deleteContentItem(contentItemId);
                    }
                });
            }

            if (plugin.settings.cancelButtonElementClass) {
                $editorPlaceholder.on("click", plugin.settings.cancelButtonElementClass, function () {
                    $editorPlaceholder.html("").hide();

                    plugin.concurrentEditors--;
                    plugin.setContentItemIdInUrl(null);

                    plugin.settings.cancelCallback.call(plugin, $editorPlaceholder);
                });
            }

            if (plugin.settings.contentItemIdQueryStringParameter.length > 0) {
                var id = plugin.getContentItemIdFromUrl();

                if (id && id.length > 0) plugin.loadEditor(id, $editorPlaceholder);
            }
        },

        loadEditor: function (contentItemId, $editorPlaceholder) {
            var plugin = this;

            if (!plugin.settings.allowMultipleEditors && plugin.concurrentEditors > 0) {
                alert(plugin.settings.multipleEditorsNotAllowedMessage);

                return;
            }

            var currentIdInUrl = plugin.getContentItemIdFromUrl();
            if (currentIdInUrl != contentItemId) {
                plugin.setContentItemIdInUrl(contentItemId);
            }

            $.ajax({
                url: plugin.settings.editUrl,
                data: { contentItemId: contentItemId },
                type: "GET"
            }).done(function (data) {
                if (data.Success) {
                    $editorPlaceholder.html($.parseHTML(data.EditorShape, true)).show();
                    plugin.concurrentEditors++;

                    var asyncEditorLoaderOptions = JSON.parse($("#" + data.AsyncEditorLoaderId + "options").val());
                    $("#" + data.AsyncEditorLoaderId).lombiq_AsyncEditor({
                        asyncEditorApiUrl: asyncEditorLoaderOptions.EditUrl,
                        contentType: asyncEditorLoaderOptions.ContentType,
                        initialContentItemId: asyncEditorLoaderOptions.ContentItemId,
                        defaultEditorGroupName: asyncEditorLoaderOptions.EditorGroup,
                        asyncEditorLoaderElementClass: asyncEditorLoaderOptions.AsyncEditorLoaderElementClass,
                        processingIndicatorElementClass: asyncEditorLoaderOptions.ProcessingIndicatorElementClass,
                        editorPlaceholderElementClass: asyncEditorLoaderOptions.EditorPlaceholderElementClass,
                        loadEditorActionElementClass: asyncEditorLoaderOptions.LoadEditorActionElementClass,
                        postEditorActionElementClass: asyncEditorLoaderOptions.PostEditorActionElementClass,
                        dirtyFormLeaveConfirmationText: asyncEditorLoaderOptions.DirtyFormLeaveConfirmationText,
                    });

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

        deleteContentItem: function (contentItemId) {
            var plugin = this;

            $.ajax({
                url: plugin.settings.deleteUrl,
                data: {
                    contentItemId: contentItemId,
                    __requestVerificationToken: plugin.settings.requestToken,
                },
                type: "POST"
            }).done(function (data) {
                if (data.Success) {
                    plugin.settings.deleteCallback.call(plugin, data);
                }

                if (data.ResultMessage) {
                    alert(data.ResultMessage);
                }
            });
        },

        getContentItemIdFromUrl: function () {
            var plugin = this;

            return new URI().search(true)[plugin.settings.contentItemIdQueryStringParameter];
        },

        setContentItemIdInUrl: function (contentItemId) {
            var plugin = this;

            if (plugin.settings.contentItemIdQueryStringParameter.length == 0) return;

            var uri = new URI();

            if (contentItemId) {
                uri.setSearch(plugin.settings.contentItemIdQueryStringParameter, contentItemId);
            }
            else {
                uri.removeSearch(plugin.settings.contentItemIdQueryStringParameter);
            }

            history.pushState(null, "", uri.pathname() + uri.search());
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
                $.data(this, "plugin_" + pluginName, new Plugin($(this), options));
            }

            // ... and then return the plugin instance, which might be null
            // if the plugin is not instantiated on this element and "options" is undefined.
            return $.data(this, "plugin_" + pluginName);
        });
    };
})(jQuery, window, document);