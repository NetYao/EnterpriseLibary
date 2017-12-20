(function ($) {
    $.fn.DemoPlugin = function (options) {
        var opts;
        opts = $.extend({}, $.fn.DemoPlugin.defaults, options);
        return demoPlugin();

        function demoPlugin() {
            alert("Hello," + opts.msg);
            if (opts.onChanged) {
                opts.onChanged();
            }
        }
    };

    $.fn.DemoPlugin.defaults = {
        msg: "World！",
        onChanged: null
    };
})(jQuery);