<%@ WebHandler Language="C#" Class="Fckupload" %>
using System;
using System.Collections;
using System.Web;
using System.IO;
using System.Globalization;
using System.Web.Script.Serialization;
using Newtonsoft.Json;

public class Fckupload:IHttpHandler
{

    public void ProcessRequest(HttpContext context)
    {
        const int maxSize = 5*1024*1024;


        var aspxUrl = context.Request.Path.Substring(0,
                                                     context.Request.Path.LastIndexOf("/", StringComparison.Ordinal) +
                                                     1);
        //文件保存目录路径
        const string savePath = "attached/";
        //文件保存目录URL
        var saveUrl = aspxUrl + savePath;
        string ckEditorFuncNum = context.Request["CKEditorFuncNum"];
        //定义允许上传的文件扩展名
        var extTable = new Hashtable
            {
                {"image", "gif,jpg,jpeg,png,bmp"},
                {"flash", "swf,flv"},
                {"media", "swf,flv,mp3,wav,wma,wmv,mid,avi,mpg,asf,rm,rmvb"},
                {"file", "doc,docx,xls,xlsx,ppt,htm,html,txt,zip,rar,gz,bz2"}
            };
        //最大文件大小



        //HttpPostedFile uploads = context.Request.Files["upload"];
        var imgFile = context.Request.Files;


        if (imgFile.Count < 1)
        {
            showError("请选择要上传的图片.", context);
        }

        var dirPath = context.Server.MapPath(savePath);
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        const string dirName = "image";
        if (imgFile.Count > 0)
        {
            var fileName = imgFile[0].FileName;
            var extension = Path.GetExtension(fileName);
            if (extension != null)
            {
                var fileExt = extension.ToLower();
                if (String.IsNullOrEmpty(fileExt) ||
                   Array.IndexOf(((String)extTable[dirName]).Split(','), fileExt.Substring(1).ToLower()) == -1)
                {
                    showError("上传图片扩展名是不允许的扩展名。只允许" + ((String)extTable[dirName]) + "格式。", context);
                }
                
                if (imgFile[0].InputStream.Length > maxSize)
                {
                    // 修改配置文件 <httpRuntime maxRequestLength="1048576" executionTimeout="3600" /> 不然会报错.
                    showError("上传图片大小超过限制5M。", context);
                }
                
               
                //创建文件夹
                dirPath += dirName + "/";
                saveUrl += dirName + "/";
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                var ymd = DateTime.Now.ToString("yyyyMMdd", DateTimeFormatInfo.InvariantInfo);
                dirPath += ymd + "/";
                saveUrl += ymd + "/";
                if (!Directory.Exists(dirPath))
                {
                    Directory.CreateDirectory(dirPath);
                }
                var newFileName = DateTime.Now.ToString("yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo) +
                                  fileExt;
                var filePath = dirPath + newFileName;
                imgFile[0].SaveAs(filePath);
                var fileUrl = saveUrl + newFileName;

                context.Response.AddHeader("Content-Type", "text/html; charset=UTF-8");
                context.Response.Write(
                    "<script type=\"text/javascript\">window.parent.CKEDITOR.tools.callFunction(" + ckEditorFuncNum +
                    ", \"" + fileUrl + "\");</script>");
                context.Response.End();

            }
        }

    }

    private void showError(string message, HttpContext context)
    {
        context.Response.AddHeader("Content-Type", "text/html; charset=UTF-8");
        //context.Response.Write("上传失败." + message);
        string ckEditorFuncNum = context.Request["CKEditorFuncNum"];
        context.Response.Write("<script type=\"text/javascript\">window.parent.CKEDITOR.tools.callFunction(" + ckEditorFuncNum + ", '', '" + message + "');</script>");
        context.Response.End();
    }

    public bool IsReusable
    {
        get
        {
            return true;
        }
    }
}
