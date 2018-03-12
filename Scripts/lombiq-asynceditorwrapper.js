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
        // The id of the ul element of the list.
        listId: "",
        // The placeholder element for the new editor which will be get async.
        // Place this anywhere outside of the list.
        newEditorPlaceholderId: "",
        // The element ID of which element click event will get the new editor.
        addNewItemElementId: "",
        // The link of the GET action which will respond with an empty editor.
        getEmptyEditorLink: "",
        // The link of the GET action which will respond with a filled editor, the route must be
        // accept an optional parameter in the end of the url,
        // because the logic will append a "/id" at the end of the url.
        getEditorLink: "",
        manageItemErrorText: "",
        // The element ID will be generated dynamically with the content item ID in the end.
        editEditorPlaceholderIdBase: "",
        // The element ID will be generated dynamically with the content item ID in the end.
        listItemIdBase: "",
        // All element with this class will store the content item ID in a "data-contentItemId"-like attribute.
        // If left empty, then the edit events won't be initialized.
        editListItemElementClass: "",
        // The class or classes that will be added to the element that wraps the markup of the successfully created content item.
        // It should be the same as what's rendered for other list items.
        listItemClass: "",
        // The cancel button class of the add new item editor.
        newItemCancelButton: "",
        // The cancel button class of the edit item editor.
        editItemCancelButton: "",
        // The data attribute name which stores the content item ID on the edit item element.
        itemDataAttributeName: "",
        // Defines whether editing multiple items at the same time is allowed or not.
        allowMultipleEditors: true,
        // The default alert to display when the user attempts to edit more than items at the same time, while it's not allwed.
        multipleEditorsNotAllowedMessage: "Editing multiple items at the same is not allowed. Please save or cancel your current changes first.",
        // If true, then redirects to the newly created content item. The content item URL must be in the ContentToDisplay property.
        redirectOnSuccessfulCreate: false,
        // Called after a successful item creation.
        onCreateSuccess: this.defaultOnCreateSuccess,
        // Called after a successful item save.
        onSaveSuccess: this.defaultOnSaveSuccess,
        // Called after the edit element events needs to be initialized.
        initEditElementEvents: this.defaultInitEditElementEvents,
        // Called after an edit element event needs to be initialized.
        initEditElementEvent: this.defaultInitEditElementEvent,
        // Called after an edit editor is successfully retrieved and inserted in the DOM.
        getItemEditEditorSuccessCustomEvents: function () { },
        // Called after an edit editor is successfully retrieved and inserted in the DOM.
        getItemCreateEditorSuccessCustomEvents: function () { },
        // Loads an empty editor on init.
        loadEditorOnInit: false,
        // Defines if there is an additional editor.
        additionalEditor: false,
        // The id of the additional editor.
        additionalEditorId: ""
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

            if (plugin.options.loadEditorOnInit) {
                plugin.getItemEditor(0, plugin.options.newEditorPlaceholderId);
            }

            $("#" + plugin.options.addNewItemElementId).on("click", function () {
                plugin.getItemEditor(0, plugin.options.newEditorPlaceholderId);
            });

            if (typeof plugin.options.initEditElementEvents == "function") {
                plugin.options.initEditElementEvents.call(plugin);
            }
        },

        getItemEditor: function (itemId, itemEditorPlaceholderId) {
            var plugin = this;

            if (!plugin.options.allowMultipleEditors && plugin.concurrentEditors > 0) {
                alert(plugin.options.multipleEditorsNotAllowedMessage);

                return;
            }

            var isCreateEditor = itemId == 0;
            $.ajax({
                url: isCreateEditor
                    ? plugin.options.getEmptyEditorLink
                    : plugin.options.getEditorLink + "/" + itemId.toString(),
                type: "GET"
            }).success(function (data) {
                var placeholderElement = $("#" + itemEditorPlaceholderId);
                placeholderElement.html(data);
                plugin.concurrentEditors++;

                if (isCreateEditor) {
                    plugin.initFormEvents(itemEditorPlaceholderId, itemId);

                    plugin.options.getItemCreateEditorSuccessCustomEvents.call(plugin, itemId);
                    // Adding event for removing additional create form.
                    if (plugin.options.additionalEditor) {
                        $("#" + plugin.options.additionalEditorId).html("");
                    }
                } else {
                    $("#" + plugin.options.listItemIdBase + "-" + itemId.toString()).hide();
                    plugin.initFormEvents(itemEditorPlaceholderId, itemId);

                    plugin.options.getItemEditEditorSuccessCustomEvents.call(plugin, itemId);
                }

                $("html, body").animate({
                    scrollTop: placeholderElement.offset().top
                }, 500);
            }).error(function (error) {
                $("#" + itemEditorPlaceholderId).html(plugin.options.manageItemErrorText);
            });
        },

        initFormEvents: function (itemEditorPlaceholderId, itemId) {
            var plugin = this;

            var isCreateEditor = itemId == 0;
            // Select the form inside the placeholder div.
            var $editorForm = $("#" + itemEditorPlaceholderId + " form");
            $editorForm.on("submit", function (event) {
                event.preventDefault();

                $.ajax({
                    type: "POST",
                    url: $editorForm.attr("action"),
                    data: new FormData($editorForm[0]),
                    async: false,
                    cache: false,
                    contentType: false,
                    processData: false,
                    // The data will be a ItemEditorAsyncPostResult with 4 properties:
                    // Id, ContentToDisplay, SuccessMessage, SuccessfulSave.
                    success: function (data) {
                        if (isCreateEditor) {
                            if (data.SuccessfulSave) {
                                if (typeof plugin.options.onCreateSuccess == "function") {
                                    plugin.options.onCreateSuccess.call(plugin, data, itemEditorPlaceholderId);
                                }

                                if (typeof plugin.options.initEditElementEvent == "function") {
                                    plugin.options.initEditElementEvent.call(plugin, data.Id);
                                }
                            } else {
                                $("#" + itemEditorPlaceholderId).html(data.ContentToDisplay);
                            }
                        } else {
                            if (data.SuccessfulSave) {
                                if (typeof plugin.options.onSaveSuccess == "function") {
                                    plugin.options.onSaveSuccess.call(plugin, data, itemEditorPlaceholderId, itemId);
                                }

                                if (typeof plugin.options.initEditElementEvent == "function") {
                                    plugin.options.initEditElementEvent.call(plugin, data.Id);
                                }
                            } else {
                                $("#" + itemEditorPlaceholderId).html(data.ContentToDisplay);
                            }
                        }

                        // Reinitializing the events is neccessary because of the async reloads.
                        plugin.initFormEvents(itemEditorPlaceholderId, itemId);
                    },
                    error: function (error) {
                        $("#" + itemEditorPlaceholderId).html(plugin.options.manageItemErrorText);
                    }
                });

                plugin.concurrentEditors--;
            });

            if (isCreateEditor) {
                // Adding event for the add new item cancel button.
                $editorForm.find("." + plugin.options.newItemCancelButton).on("click", function () {
                    $("#" + itemEditorPlaceholderId).html("");
                    plugin.concurrentEditors--;
                });
            } else {
                // Adding event for the edit item cancel buttons.
                $editorForm.find("." + plugin.options.editItemCancelButton).on("click", function () {
                    $("#" + itemEditorPlaceholderId).html("");
                    $("#" + plugin.options.listItemIdBase + "-" + itemId.toString()).show();
                    plugin.concurrentEditors--;
                });
            }
        },

        defaultOnCreateSuccess: function (data, itemEditorPlaceholderId) {
            var plugin = this;

            if (plugin.options.redirectOnSuccessfulCreate) {
                window.location.href = data.ContentToDisplay;
            } else {
                // Inserting item to the top of the list.
                var $firstListItem = $("#" + plugin.options.listId + " li:eq(0)");
                var htmlElementToInsert = "<li class=\"" + plugin.options.listItemClass + "\">" + data.ContentToDisplay + "</li>";
                if ($firstListItem.length == 0) {
                    $("#" + plugin.options.listId).html(htmlElementToInsert);
                } else {
                    $firstListItem.before(htmlElementToInsert);
                }

                // Displaying success message.
                $("#" + itemEditorPlaceholderId).html(data.SuccessMessage);
            }
        },

        defaultOnSaveSuccess: function (data, itemEditorPlaceholderId, itemId) {
            var plugin = this;

            $("#" + itemEditorPlaceholderId).remove();
            $("#" + plugin.options.listItemIdBase + "-" + itemId.toString()).replaceWith(data.ContentToDisplay).show();
        },

        // Initializes the click events for the edit item elements.
        defaultInitEditElementEvents: function () {
            var plugin = this;

            if (plugin.options.editListItemElementClass) {
                $("." + plugin.options.editListItemElementClass).on("click", function () {
                    var itemId = $(this).attr(plugin.options.itemDataAttributeName);
                    if (itemId > 0) {
                        plugin.getItemEditor(itemId, plugin.options.editEditorPlaceholderIdBase + "-" + itemId.toString());
                    }
                });
            }
        },

        // Initializes the click event for one edit item element.
        defaultInitEditElementEvent: function (itemId) {
            var plugin = this;

            if (plugin.options.editListItemElementClass) {
                $("#" + plugin.options.editListItemElementClass + "-" + itemId).on("click", function () {
                    plugin.getItemEditor(itemId, plugin.options.editEditorPlaceholderIdBase + "-" + itemId.toString());
                });
            }
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

    $[pluginName] = staticVariables;
})(jQuery, window, document);