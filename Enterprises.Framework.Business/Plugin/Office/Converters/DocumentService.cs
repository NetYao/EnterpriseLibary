using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

namespace Enterprises.Framework.Plugin.Office.Converters
{
    /// <summary>
    ///  文档保存数据库服务
    /// </summary>
    public class DocumentService
    {
        private static readonly DocumentService _instance;

        private DocumentService() { }

        static DocumentService()
        {
            _instance = new DocumentService();
        }

        public static DocumentService Instance
        {
            get
            {
                return _instance;
            }
        }

        public Guid AddDocument(DocumentEntity model)
        {
            model.CreateTime = DateTime.Now;
            return DocumentDal.Insert(model);
        }

        public DocumentEntity UpdateDocument(DocumentEntity model)
        {
            int result = DocumentDal.Update(model);
            if (result > 0)
            {
                return model;
            }

            return null;
        }

        public DocumentEntity GetDocument(Guid id)
        {
            return DocumentDal.GetDocument(id);
        }

        public List<DocumentEntity> GetDocument(DocumentStatus status)
        {
            return DocumentDal.GetDocument(status);
        }

        public string GetDocumentLocation(Guid id)
        {
            DocumentEntity doc = GetDocument(id);

            if (doc != null && doc.Status == (int)DocumentStatus.Processed)
            {
                switch (doc.OutputType)
                {
                    case (int)OutputType.Word:
                        return string.Format("{0}.docx", doc.DocumentAddress);
                    case (int)OutputType.PDF:
                        return string.Format("{0}.pdf", doc.DocumentAddress);
                    case (int)(OutputType.PDF| OutputType.Word):
                        return string.Format("{0}.docx,{0}.pdf", doc.DocumentAddress);
                    default:
                        break;
                }
            }
            return string.Empty;
        }
    }
}

