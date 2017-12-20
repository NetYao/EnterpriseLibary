/*----------------------------------------------------------------------------
--功能说明:   %H_DESC%

--创建时间:   %H_DATE%

--其它信息:   此文件自动生成,并依赖json2.js <http://www.JSON.org/json2.js>

--内核维护:   wzcheng@iflytek.com 
------------------------------------------------------------------------------*/

(function($) {

    if (!$.net) {

        var defaultOptions = { contentType: "application/json; charset=utf-8"
                , dataType: "json"
                , type: "POST"
              //, complete: function(r, status) { debugger; } //此代码加上用于全局调试
        };

        //将net作为命名空间扩展到jQuery框架内
        $.extend({ net: {} });

        //将调用WEB SERVICES的代理函数CallWebMethod扩展到jQuery.svr框架内
        $.extend($.net, {

            CallWebMethod: function(options, method, args, callback) {

                //调用第三方对象序列化成JSON字符串的方法
                var jsonStr = JSON.stringify(args);

                var parameters = $.extend({}, defaultOptions);

                var url0 = options.url + "/" + method;

                $.extend(parameters, options, { url: url0, data: jsonStr, success: callback });

                $.ajax(parameters);
            }
        });
    }

    //将指定类型的WEB服务扩展到jQuery框架内
    var services = new %CLS%();
    $.extend($.net, { %CLS%: services });

})(jQuery);



/*----------------------------------------------------------------------------
--功能说明: 服务的构造函数
----------------------------------------------------------------------------*/
function %CLS%() {

    /*
    --定义本地的调用选项，如果希望改变个别的ajax调用选项，
    --请在对象中添加其它选项的键/值
    */
    this.Options = { url: "%URL%" };

}
 
//以下为系统公开的可调用方法

