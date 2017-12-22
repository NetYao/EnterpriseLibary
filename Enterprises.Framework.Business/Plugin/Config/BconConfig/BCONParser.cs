using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enterprises.Framework.Plugin.Config.BconConfig
{
    /// <summary>
    /// BCON 格式化转换
    /// </summary>
    public static class BCONParser
    {
        /// <summary>
        /// Parse BCON format configuration string.
        /// </summary>
        public static dynamic ParseBCON(this string text, BCONConfig config = null)
        {
            return ParseBCON(text.Split(new[] { '\r', '\n' }), config);
        }

        /// <summary>
        /// Parse BCON format configuration stream.
        /// </summary>
        public static dynamic ParseBCON(this Stream stream, BCONConfig config = null)
        {
            using (var reader = new StreamReader(stream))
            {
                return ParseBCON(reader.ReadToEnd(), config);
            }
        }

        private static dynamic ParseBCON(IEnumerable<string> stream, BCONConfig config)
        {
            dynamic result = new ExpandoObject();

            if (config == null)
            {
                config = BCONConfig.DefaultConfig;
            }

            string currentPropertyName = string.Empty;
            List<dynamic> values = null;
            foreach (var line in stream)
            {
                if (string.IsNullOrEmpty(line) || line.TrimStart().StartsWith(config.CommentChars))
                {
                    continue;
                }

                if (line.Trim().Contains(config.PropertyStartChar))
                {
                    string propertyName = ExtractPropertyName(line.Trim(), config);

                    if (!string.IsNullOrEmpty(propertyName))
                    {
                        currentPropertyName = propertyName;
                        values = new List<dynamic>();
                    }

                    continue;
                }

                if (!string.IsNullOrEmpty(currentPropertyName) && values != null)
                {
                    values.Add(ExtractValue(line.Trim(), config));
                    (result as IDictionary<string, dynamic>)[currentPropertyName] = values.Count == 1 ? values.First() : values;
                }
            }

            return result;
        }

        /// <summary>
        /// Extract value, include extract "Property=Value" formay
        /// </summary>
        private static dynamic ExtractValue(string line, BCONConfig config)
        {
            if (config.ValueSeperator.Any(line.Contains) || config.PropertySeperator.Any(line.Contains))
            {
                dynamic obj = new ExpandoObject();
                string[] parameters = line.Split(config.PropertySeperator, StringSplitOptions.RemoveEmptyEntries);
                foreach (string p in parameters)
                {
                    string[] values = p.Split(config.ValueSeperator, StringSplitOptions.RemoveEmptyEntries);
                    if (values.Length > 1)
                    {
                        (obj as IDictionary<string, dynamic>)[values[0].Trim()] = ExtractValue(values[1].Trim(), config);
                    }
                }

                return obj;
            }

            double doubleValue;
            int intValue;
            if (double.TryParse(line, NumberStyles.Number, null, out doubleValue))
            {
                return doubleValue;
            }


            if (int.TryParse(line, NumberStyles.Number, null, out intValue))
            {
                return intValue;
            }

            return line;
        }

        /// <summary>
        /// Extract property name.
        /// </summary>
        private static string ExtractPropertyName(string line, BCONConfig config)
        {
            int startIndex = line.IndexOf(config.PropertyStartChar, StringComparison.Ordinal);
            int endIndex = line.IndexOf(config.PropertyEndChar, StringComparison.Ordinal);

            if (startIndex < 0 || endIndex < 0 || endIndex <= startIndex)
            {
                return string.Empty;
            }

            return line.Substring(startIndex + 1, endIndex - startIndex - 1).Trim();
        }
    }
}
