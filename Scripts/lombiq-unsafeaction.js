/**
 * @summary     Lombiq - Unsafe Action
 * @description Wraps an unsafe action (i.e. HTTP POST action link or a remove link) with a form and posts it to the server.
 * @version     1.0
 * @file        lombiq-unsafeaction.js
 * @author      Lombiq Technologies Ltd.
 */

; (function ($, window, document, undefined) {
    "use strict";

    var pluginName = "lombiq_UnsafeAction";

    // Define static variables accessible using the plugin name as an object.
    var staticVariables = {
        confirmationRequested: pluginName + "_ConfirmationRequested"
    };

    var defaults = {
        confirmationMessage: "",
        confirmationRequired: false,
        autoTrigger: false,
        requestToken: "",
        confirmationCallback: function (plugin) { }
    };

    function Plugin(element, options) {
        this.element = element;
        this.settings = $.extend(true, {}, defaults, options);
        this._defaults = defaults;
        this._name = pluginName;

        this.init();
    }

    $.extend(Plugin.prototype, {
        $actionLink: null,
        requestToken: "",

        init: function () {
            var plugin = this;

            plugin.$actionLink = $(plugin.element);
            plugin.requestToken = plugin.settings.requestToken.length > 0 ?
                plugin.settings.requestToken :
                $("input[name=__RequestVerificationToken]").first().val();

            if (!plugin.requestToken) return;

            plugin.$actionLink.on("click", function (e) {
                plugin.triggerUnsafeAction();

                return false;
            });

            if (plugin.settings.autoTrigger) {
                plugin.triggerUnsafeAction();
            }
        },

        triggerUnsafeAction: function () {
            var plugin = this;

            if (!plugin.settings.confirmationRequired) {
                plugin.executeUnsafeAction();
            }
            else if (plugin.settings.confirmationMessage) {
                if (confirm(plugin.settings.confirmationMessage)) {
                    plugin.executeUnsafeAction();
                }
            }
            else {
                // True value of confirmationRequired without message means that the confirmation is handled in the
                // confirmation callback. If that is the case, the executeUnsafeAction() must be called directly.

                plugin.settings.confirmationCallback(plugin);

                plugin.$actionLink.trigger($.lombiq_UnsafeAction.confirmationRequested, [plugin]);
            }
        },

        executeUnsafeAction: function () {
            var plugin = this;

            var hrefParts = plugin.$actionLink.attr("href").split("?");
            var $form = $("<form action=\"" + hrefParts[0] + "\" method=\"POST\" />");
            var $requestToken = $("<input name=\"__RequestVerificationToken\" value=\"" + plugin.requestToken + "\" />");

            $form.append($requestToken);

            if (hrefParts.length > 1) {
                var queryParts = hrefParts[1].split("&");

                for (var i = 0; i < queryParts.length; i++) {
                    var queryPartKVP = queryParts[i].split("=");

                    $form.append($("<input type=\"hidden\" name=\"" +
                        decodeURIComponent(queryPartKVP[0]) +
                        "\" value=\"" +
                        decodeURIComponent(queryPartKVP[1]) +
                        "\" />"));
                }
            }

            $form.css({ "position": "absolute", "left": "-9999em" });
            $("body").append($form);

            $form.submit();
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


    $(function () {
        $(document).on("click", "a[itemprop~=UnsafeAction], a[data-unsafe-action]", function () {
            var $self = $(this);

            var confirmationMessage = $self.data("confirmationMessage") || "";
            var options = {
                autoTrigger: true,
                confirmationMessage: confirmationMessage,
                confirmationRequired: $self.data("confirmationRequired") || (confirmationMessage.length > 0 ? true : false),
                requestToken: $self.data("requestToken") || "",
            }

            $self.lombiq_UnsafeAction(options);

            return false;
        });
    });
})(jQuery, window, document);