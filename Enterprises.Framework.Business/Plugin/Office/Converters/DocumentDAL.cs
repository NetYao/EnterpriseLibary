using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;

namespace Enterprises.Framework.Plugin.Office.Converters
{
    /* 在目标数据库创建表
     CREATE TABLE [dbo].[Document](
	[ID] [uniqueidentifier] NOT NULL,
	[TemplateName] [nvarchar](200) NOT NULL,
	[OutputType] [int] NOT NULL,
	[DataSource] [nvarchar](max) NOT NULL,
	[DocumentAddress] [nvarchar](200) NULL,
	[Status] [int] NOT NULL,
	[InfoMessage] [nvarchar](max) NULL,
	[CreateTime] [datetime] NOT NULL,
     CONSTRAINT [PK_Document] PRIMARY KEY CLUSTERED 
    (
	    [ID] ASC
    )WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
    ) ON [PRIMARY]
     */
    /// <summary>
    ///  文档保存数据库操作
    /// </summary>
    
    public class DocumentDal
    {
        private static readonly string ConnectionString;
        private static readonly Exception Exception;

        static DocumentDal()
        {
            try
            {
                ConnectionString = DocumentConvertConfig.ConnectionString;
            }
            catch(Exception ex)
            {
                Exception = ex;
            }
        }

        public static Guid Insert(DocumentEntity doc)
        {
            if (Exception != null)
            {
                throw Exception;
            }

            const string sql = @"INSERT INTO dbo.DocumentEntity (ID,TemplateName,OutputType,DataSource,DocumentAddress,
                                                     [Status],InfoMessage,CreateTime)Values(@ID,@TemplateName,
                                                      @OutputType,@DataSource,@DocumentAddress,@Status,@InfoMessage,
                                                     @CreateTime)";
            doc.ID = Guid.NewGuid();
            int result = 0;
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (var cmd = new SqlCommand(sql, conn))
                {
                    var parameter = new SqlParameter("@ID", DbType.Guid) {Value = doc.ID};
                    cmd.Parameters.Add(parameter);
                    parameter = new SqlParameter("@TemplateName", DbType.String) {Value = doc.TemplateName};
                    cmd.Parameters.Add(parameter);
                    parameter = new SqlParameter("@OutputType", DbType.Int32) {Value = doc.OutputType};
                    cmd.Parameters.Add(parameter);
                    parameter = new SqlParameter("@DataSource", DbType.String) {Value = doc.DataSource};
                    cmd.Parameters.Add(parameter);
                    parameter = new SqlParameter("@DocumentAddress", DbType.String) {Value = string.Empty};
                    cmd.Parameters.Add(parameter);
                    parameter = new SqlParameter("@Status", DbType.Int32) {Value = (int) DocumentStatus.New};
                    cmd.Parameters.Add(parameter);
                    parameter = new SqlParameter("@InfoMessage", DbType.String) {Value = string.Empty};
                    cmd.Parameters.Add(parameter);
                    parameter = new SqlParameter("@CreateTime", DbType.DateTime) {Value = DateTime.Now};
                    cmd.Parameters.Add(parameter);
                    result = cmd.ExecuteNonQuery();
                }
            }
            if (result > 0)
            {
                return doc.ID;
            }
            throw new ApplicationException("文档持久化失败！");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="doc"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static int Update(DocumentEntity doc)
        {
            if (Exception != null)
            {
                throw Exception;
            }
            string sql = @"UPDATE dbo.Document SET TemplateName=@TemplateName,OutputType=@OutputType,
                           DataSource=@DataSource, [Status]=@Status,InfoMessage=@InfoMessage,DocumentAddress=@DocumentAddress WHERE ID=@ID";
            int result = 0;
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(sql, conn))
                {
                    SqlParameter parameter = new SqlParameter("@ID", DbType.Guid);
                    parameter.Value = doc.ID;
                    cmd.Parameters.Add(parameter);
                    parameter = new SqlParameter("@TemplateName", DbType.String);
                    parameter.Value = doc.TemplateName;
                    cmd.Parameters.Add(parameter);
                    parameter = new SqlParameter("@OutputType", DbType.Int32);
                    parameter.Value = doc.OutputType;
                    cmd.Parameters.Add(parameter);
                    parameter = new SqlParameter("@DataSource", DbType.String);
                    parameter.Value = doc.DataSource;
                    cmd.Parameters.Add(parameter);
                    parameter = new SqlParameter("@Status", DbType.Int32);
                    parameter.Value = doc.Status;
                    cmd.Parameters.Add(parameter);
                    parameter = new SqlParameter("@InfoMessage", DbType.String);
                    parameter.Value = doc.InfoMessage;
                    cmd.Parameters.Add(parameter);
                    parameter = new SqlParameter("@DocumentAddress", DbType.String);
                    parameter.Value = doc.DocumentAddress??"";
                    cmd.Parameters.Add(parameter);
                    result = cmd.ExecuteNonQuery();
                }
            }
            return result;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static List<DocumentEntity> GetDocument(DocumentStatus status = DocumentStatus.None)
        {
            if (Exception != null)
            {
                throw Exception;
            }
            string condition = string.Empty;

            switch (status)
            {
                case DocumentStatus.Processed:
                    condition = "WHERE [Status] = 1";
                    break;
                case DocumentStatus.New:
                    condition = "WHERE  [Status] = 2";
                    break;
                case DocumentStatus.Modified:
                    condition = "WHERE  [Status] = 4";
                    break;
                case DocumentStatus.Failed:
                    condition = "WHERE [Status]=8";
                    break;
                case DocumentStatus.New | DocumentStatus.Modified:
                    condition = "WHERE [Status] IN (2,4)";
                    break;
                case DocumentStatus.New | DocumentStatus.Modified | DocumentStatus.Failed:
                    condition = "WHERE [Status] IN (2,4,8)";
                    break;
                default:
                    break;
            }

            string sql = string.Format("SELECT * FROM dbo.DocumentEntity {0}", condition);
            List<DocumentEntity> list = new List<DocumentEntity>();
            using (SqlConnection connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(sql, connection))
                {
                    using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        while (reader.Read())
                        {
                            DocumentEntity doc = GetDcoument(reader);
                            list.Add(doc);
                        }
                    }
                }
            }
            return list;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        public static DocumentEntity GetDocument(Guid id)
        {
            if (Exception != null)
            {
                throw Exception;
            }

            const string sql = "SELECT * FROM dbo.DocumentEntity WHERE ID=@ID";
            DocumentEntity doc = null;
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                using (var cmd = new SqlCommand(sql, connection))
                {
                    var parameter = new SqlParameter("@ID", DbType.Guid);
                    parameter.Value = id;
                    cmd.Parameters.Add(parameter);
                    using (SqlDataReader reader = cmd.ExecuteReader(CommandBehavior.CloseConnection))
                    {
                        if (reader.Read())
                        {
                            doc = GetDcoument(reader);
                        }
                    }
                }
            }
            return doc;
        }

        private static DocumentEntity GetDcoument(SqlDataReader reader)
        {
            var doc = new DocumentEntity();
            int index = 0;
            doc.ID = reader.GetGuid(reader.GetOrdinal("ID"));
            doc.OutputType = reader.GetInt32(reader.GetOrdinal("OutputType"));
            doc.Status = reader.GetInt32(reader.GetOrdinal("Status"));
            doc.TemplateName = reader.GetString(reader.GetOrdinal("TemplateName"));
            index = reader.GetOrdinal("DocumentAddress");
            doc.DocumentAddress = reader.IsDBNull(index) ? "" : reader.GetString(index);
            doc.DataSource = reader.GetString(reader.GetOrdinal("DataSource"));
            index = reader.GetOrdinal("InfoMessage");
            doc.InfoMessage = reader.IsDBNull(index) ? "" : reader.GetString(index);
            doc.CreateTime = reader.GetDateTime(reader.GetOrdinal("CreateTime"));
            return doc;
        }
    }
}
