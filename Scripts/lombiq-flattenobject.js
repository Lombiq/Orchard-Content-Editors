; (function ($, window, document, undefined) {
    $(function () {
        $.fn.extend({
            // Credits for https://stackoverflow.com/a/53739792/2883185.
            flattenObject:  function(ob) {
                var toReturn = {};

                for (var i in ob) {
                    if (!ob.hasOwnProperty(i)) continue;

                    if ((typeof ob[i]) == 'object' && ob[i] !== null) {
                        var flatObject = $.fn.flattenObject(ob[i]);
                        for (var x in flatObject) {
                            if (!flatObject.hasOwnProperty(x)) continue;

                            toReturn[i + '.' + x] = flatObject[x];
                        }
                    } else {
                        toReturn[i] = ob[i];
                    }
                }
                return toReturn;
            }
        });
    });
})(jQuery, window, document);