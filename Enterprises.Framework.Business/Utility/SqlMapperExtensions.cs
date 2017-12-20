using System.Collections.Generic;
using System.Linq;

namespace Enterprises.Framework.Utility
{
    public interface IConciseSqlBuilder
    {
        string BuildInsert(string table, List<string> properties);

        string BuildInsertEffact(string table, List<string> properties);

        string BuildUpdate(string table, List<string> updateProperties, List<string> whereProperties);

        string BuildDelete(string table, List<string> whereProperties);

        string BuildCount(string table, List<string> properties, bool isOr = false);

        string BuildQuerySql(string table, List<string> properties, string selectPart = "*", bool isOr = false);

        string BuildQueryPaged(string table, List<string> properties, string orderBy, int pageIndex, int pageSize,
            string columns = "*", bool isOr = false);

    }


    public class SqlServerConciseBuilder : IConciseSqlBuilder
    {


        public string BuildInsert(string table, List<string> properties)
        {
            var columns = string.Join(",", properties.Select(x => $"[{x}]"));
            var values = string.Join(",", properties.Select(p => "@" + p));
            return $"INSERT INTO {table} ({columns}) VALUES ({values}) SELECT CAST(SCOPE_IDENTITY() AS BIGINT)";
        }

        public string BuildInsertEffact(string table, List<string> properties)
        {
            var columns = string.Join(",", properties.Select(x => $"[{x}]"));
            var values = string.Join(",", properties.Select(p => "@" + p));
            return $"INSERT INTO {table} ({columns}) VALUES ({values})";
        }

        public string BuildUpdate(string table, List<string> updateProperties, List<string> whereProperties)
        {
            var updateFields = string.Join(",", updateProperties.Select(p => $"[{p}]" + " = @" + p));
            var whereFields = string.Empty;
            if (whereProperties.Any())
            {
                whereFields = " WHERE " + string.Join(" AND ", whereProperties.Select(p => $"[{p}]" + " = @w_" + p));
            }
            return $"UPDATE {table} SET {updateFields}{whereFields}";
        }

        public string BuildDelete(string table, List<string> whereProperties)
        {
            var whereFields = string.Empty;
            if (whereProperties.Count > 0)
            {
                whereFields = " WHERE " + string.Join(" AND ", whereProperties.Select(p => $"[{p}]" + " = @" + p));
            }
            return $"DELETE FROM {table}{whereFields}";
        }

        public string BuildCount(string table, List<string> properties, bool isOr = false)
        {
            if (properties.Count == 0)
            {
                return $"SELECT COUNT(*) FROM {table}";
            }
            var separator = isOr ? " OR " : " AND ";
            var wherePart = string.Join(separator, properties.Select(p => $"[{p}]" + " = @" + p));
            return $"SELECT COUNT(*) FROM {table} WHERE {wherePart}";
        }

        public string BuildQuerySql(string table, List<string> properties, string selectPart = "*", bool isOr = false)
        {
            if (properties.Count == 0)
            {
                return $"SELECT {selectPart} FROM {table}";
            }
            var separator = isOr ? " OR " : " AND ";
            var wherePart = string.Join(separator, properties.Select(p => $"[{p}]" + " = @" + p));
            return $"SELECT {selectPart} FROM {table} WHERE {wherePart}";
        }

        public string BuildQueryPaged(string table, List<string> properties, string orderBy, int pageIndex, int pageSize,
            string columns = "*", bool isOr = false)
        {
            var whereFields = string.Empty;
            if (properties.Count > 0)
            {
                var separator = isOr ? " OR " : " AND ";
                whereFields = " WHERE " + string.Join(separator, properties.Select(p => $"[{p}]" + " = @" + p));
            }
            return
                $"SELECT {columns} FROM (SELECT ROW_NUMBER() OVER (ORDER BY {orderBy}) AS RowNumber, {columns} FROM {table}{whereFields}) AS Total WHERE RowNumber >= {(pageIndex - 1) * pageSize + 1} AND RowNumber <= {pageIndex * pageSize}";
        }
    }


    public class MySqlConciseBuilder : IConciseSqlBuilder
    {
        public string BuildInsert(string table, List<string> properties)
        {
            var columns = string.Join(",", properties.Select(x => $"`{x}`"));
            var values = string.Join(",", properties.Select(p => "@" + p));
            return $"INSERT INTO {table} ({columns}) VALUES ({values}); SELECT LAST_INSERT_ID()";
        }

        public string BuildInsertEffact(string table, List<string> properties)
        {
            var columns = string.Join(",", properties.Select(x => $"`{x}`"));
            var values = string.Join(",", properties.Select(p => "@" + p));
            return $"INSERT INTO {table} ({columns}) VALUES ({values})";
        }

        public string BuildUpdate(string table, List<string> updateProperties, List<string> whereProperties)
        {
            var updateFields = string.Join(",", updateProperties.Select(p => $"`{p}`" + " = @" + p));
            var whereFields = string.Empty;
            if (whereProperties.Any())
            {
                whereFields = " WHERE " + string.Join(" and ", whereProperties.Select(p => $"`{p}`" + " = @w_" + p));
            }
            return $"UPDATE {table} SET {updateFields}{whereFields}";
        }

        public string BuildDelete(string table, List<string> whereProperties)
        {
            var whereFields = string.Empty;
            if (whereProperties.Count > 0)
            {
                whereFields = " WHERE " + string.Join(" AND ", whereProperties.Select(p => $"`{p}`" + " = @" + p));
            }
            return $"DELETE FROM {table}{whereFields}";
        }

        public string BuildCount(string table, List<string> properties, bool isOr = false)
        {
            if (properties.Count == 0)
            {
                return $"SELECT COUNT(*) FROM {table}";
            }
            var separator = isOr ? " OR " : " AND ";
            var wherePart = string.Join(separator, properties.Select(p => $"`{p}`" + " = @" + p));
            return $"SELECT COUNT(*) FROM {table} WHERE {wherePart}";
        }


        public string BuildQuerySql(string table, List<string> properties, string selectPart = "*", bool isOr = false)
        {
            if (properties.Count == 0)
            {
                return string.Format("SELECT {1} FROM {0}", table, selectPart);
            }
            var separator = isOr ? " OR " : " AND ";
            var wherePart = string.Join(separator, properties.Select(p => $"`{p}`" + " = @" + p));
            return $"SELECT {selectPart} FROM {table} WHERE {wherePart}";
        }

        public string BuildQueryPaged(string table, List<string> properties, string orderBy, int pageIndex, int pageSize,
            string columns = "*", bool isOr = false)
        {
            var whereFields = string.Empty;
            if (properties.Count > 0)
            {
                var separator = isOr ? " OR " : " AND ";
                whereFields = " WHERE " + string.Join(separator, properties.Select(p => $"`{p}`" + " = @" + p));
            }
            return
                $"SELECT {columns} FROM {table} {whereFields} ORDER BY {orderBy} LIMIT {(pageIndex - 1) * pageSize},{pageSize}";
        }
    }



}
