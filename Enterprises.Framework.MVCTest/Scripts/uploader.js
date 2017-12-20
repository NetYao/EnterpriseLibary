var cur_limitfilesize = 5 * 1024 * 1024;  // 5 MB = 5 * 1024 * 1024 bytes
function initUploader(btnid, folder, uploadHandler, deleteHandler) {
    var uploader = new qq.FineUploader({
        element: document.getElementById(btnid),
        text: {
            uploadButton: '添加文件', //'　'
            cancelButton: ''//删除
        },
        multiple: true,
        disableCancelForFormUploads: true,
        validation: {
            allowedExtensions: [], //'jpeg', 'jpg', 'gif', 'png', 'doc', 'docx'
            sizeLimit: cur_limitfilesize //5 * 1024 * 1024 // 5 MB = 5 * 1024 * 1024 bytes
        },
        dragAndDrop: {
            disableDefaultDropzone: true
        },
        request: {
            endpoint: uploadHandler,
            params: {
                "folder": folder,
                "sizeLimit": cur_limitfilesize
            },
            forceMultipart: true
        },
        showMessage: function () { },
        callbacks: {
            onValidate: function (fileData) {
                var len = fileData.length;
                len += $("#btnUpload-list > li").length;
                if (len > 5) {
                    $.facebox("最多只能上传5个附件。");
                    return false;
                }
                return true;
            },
            onSubmit: function (id, fileName) {
                this._options.request.params.sizeLimit = cur_limitfilesize;
            },
            onError: function (id, fileName, reason) {
                
            },
            onComplete: function (id, fileName, responseJSON) {
                if (responseJSON.success) { }
            },
            onCancel: function (id, fileName) {
                $.ajax({
                    type: "POST",
                    url: deleteHandler,
                    data: { "folder": $("#__AttachmentFolder").val(), "fileName": fileName },
                    success: function (data) {
                        var d = eval('(' + data + ')');
                        if (!d.success) {
                            $.fn.notify({ message: "文件删除出错，请稍候再试！" });
                        }
                    },
                    error: function (d) {
                        $.fn.notify({ message: "文件删除出错，请稍候再试！" });
                    }
                });
            }
        },
        messages: {
            typeError: "文件扩展名必须为: {extensions}。",
            sizeError: "{file} 文件太大, 最大支持 {sizeLimit}。",
            minSizeError: "{file} 文件太小, 最小支持 {minSizeLimit}。",
            emptyError: "{file} 文件不能为空。",
            noFilesError: "请选择文件。",
            onLeave: "正在上传文件，如果离开，上传将取消。"
        },
        debug: true
    });
}