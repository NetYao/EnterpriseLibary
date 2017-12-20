/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.md or http://ckeditor.com/license
 */

(function () {
    var imageDialog = function (editor, dialogType) {
        return {
            title: "上传图片",
            resizable: CKEDITOR.DIALOG_RESIZE_BOTH,
            minWidth: 420,
            minHeight: 200,
            onOk: function () {
                var src = this.getContentElement('Upload', 'txtUrl').getValue();
                if (src == null || src == "") {
                    alert('请选择上传图片');
                    return false;
                }

                this.imageElement = editor.document.createElement('img');
                this.imageElement.setAttribute('alt', '');
                this.imageElement.setAttribute('src', src);
                editor.insertElement(this.imageElement);
                return true;
            },
            onHide: function () {
                delete this.imageElement;
            },
            contents: [{
                id: 'Upload',
                filebrowser: 'uploadButton',
                label: "上传",
                elements: [
                    {
                        type: 'file',
                        id: 'upload',
                        label: "选择要上传的图片",
                        size: 50
                    },
                    {
                        type: 'fileButton',
                        id: 'uploadButton',
                        filebrowser:
                           {
                               action: 'QuickUpload',
                               target: 'Upload:txtUrl',
                               onSelect: function (fileUrl) {
                                   if (fileUrl != "") {
                                       this.imageElement = editor.document.createElement('img');
                                       this.imageElement.setAttribute('alt', '');
                                       this.imageElement.setAttribute('src', fileUrl);
                                       editor.insertElement(this.imageElement);
                                       var dialog = this.getDialog();
                                       dialog.hide();
                                   }
                               }
                           },
                        label: "上传",
                        'for': ['Upload', 'upload']
                    },
                    {
                        id: 'txtUrl',
                        type: 'text',
                        style: 'display:none;',
                        label: '图片网址'
                    }
                ]
            }]
        };
    };

    CKEDITOR.dialog.add('image', function (editor) {
        return imageDialog(editor, 'image');
    });
})();
