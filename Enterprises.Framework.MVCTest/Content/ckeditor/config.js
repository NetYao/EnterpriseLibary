/**
 * @license Copyright (c) 2003-2013, CKSource - Frederico Knabben. All rights reserved.
 * For licensing, see LICENSE.html or http://ckeditor.com/license
 */

CKEDITOR.editorConfig = function (config) {
    // Define changes to default configuration here. For example:
    config.language = 'zh-cn';
    //config.uiColor = '#AADC6E';

    config.toolbarGroups = [
		{ name: 'clipboard', groups: ['clipboard', 'undo'] },
//		{ name: 'editing', groups: ['find', 'selection', 'spellchecker'] },
//		{ name: 'links' },
//		{ name: 'others' },
//		'/',
		{ name: 'basicstyles', groups: ['basicstyles'] },
		{ name: 'paragraph', groups: ['align'] },
		{ name: 'styles' },
        { name: 'insert' },
		{ name: 'colors' },
        { name: 'document', groups: ['mode', 'document', 'doctools', 'chkFull'] },
    	{ name: 'tools' }
	];

    config.removeButtons = 'Subscript,Superscript,Iframe,Print,Flash,Smiley,Save,Templates,NewPage,Div,Format,Strike,HorizontalRule,SpecialChar,PageBreak,Table,ShowBlocks';
    config.removePlugins = 'stylescombo,sourcearea';
    config.filebrowserUploadUrl = '/Content/ckeditor/Fckupload.ashx?command=QuickUpload&type=Images';
    
};
