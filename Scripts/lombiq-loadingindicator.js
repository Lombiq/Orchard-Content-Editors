/**
 * @summary     Lombiq - Loading Indicator
 * @description Loading indicator functionality for async operations.
 * @version     1.0
 * @file        lombiq-loadingindicator.js
 * @author      Lombiq Technologies Ltd.
 */

; (function ($, window, document, undefined) {
    "use strict";

    var pluginName = "lombiq_LoadingIndicator";

    var defaults = {
        loadingIndicatorContainerClassName: "",
        minimumDisplayTimeInMilliseconds: 500,
        showCallback: function (element, plugin) { },
        hideCallback: function (element, plugin) { }
    };
    
    function Plugin(element, options) {
        this.element = element;
        this.settings = $.extend(true, {}, defaults, options);
        this._defaults = defaults;
        this._name = pluginName;

        this.init();
    }

    $.extend(Plugin.prototype, {
        $loadingIndicatorContainer: null,
        minimumTimeElapsed: false,
        hideRequested: false,

        init: function () {
            var plugin = this;

            plugin.$loadingIndicatorContainer = $(plugin.settings.loadingIndicatorContainerClassName);
        },

        show: function () {
            var plugin = this;

            if (!plugin.settings.loadingIndicatorContainerClassName) return;

            plugin.minimumTimeElapsed = false;
            plugin.hideRequested = false;

            plugin.$loadingIndicatorContainer.show();

            plugin.settings.showCallback(plugin.element, plugin);

            setTimeout(function () {
                plugin.minimumTimeElapsed = true;

                if (plugin.hideRequested) {
                    plugin.hideLoadingIndicatorElement();
                }
            }, plugin.settings.minimumDisplayTimeInMilliseconds);
        },

        hide: function () {
            var plugin = this;

            if (!plugin.settings.loadingIndicatorContainerClassName) return;

            plugin.hideRequested = true;

            if (plugin.minimumTimeElapsed) {
                plugin.hideLoadingIndicatorElement();
            }
        },

        hideLoadingIndicatorElement: function () {
            var plugin = this;

            if (!plugin.settings.loadingIndicatorContainerClassName) return;

            plugin.$loadingIndicatorContainer.hide();
            plugin.settings.hideCallback(plugin.element, plugin);
        }
    });

    var staticLoadingIndicator = {
        initializeStaticLoadingIndicator: function (options) {
            $(document).lombiq_LoadingIndicator(options);
        },

        show: function () {
            $(document).lombiq_LoadingIndicator()[0].show();
        },

        hide: function () {
            $(document).lombiq_LoadingIndicator()[0].hide();
        }
    };
    
    $.fn[pluginName] = function (options) {
        // Return null if the element query is invalid.
        if (!this || this.length === 0) return null;

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

    $[pluginName] = staticLoadingIndicator;
})(jQuery, window, document);