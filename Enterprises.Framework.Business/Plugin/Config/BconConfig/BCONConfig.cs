using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Enterprises.Framework.Plugin.Config.BconConfig
{
    /// <summary>
    /// BCON parser configuration.
    /// </summary>
    public class BCONConfig
    {
        internal static BCONConfig DefaultConfig = new BCONConfig();

        public BCONConfig(
            string commentChars = "#",
            string propertyStartChar = "[",
            string propertyEndChar = "]",
            char[] propertySeperator = null,
            char[] valueSeperator = null)
        {
            this.CommentChars = commentChars;
            this.PropertyStartChar = propertyStartChar;
            this.PropertyEndChar = propertyEndChar;
            this.PropertySeperator = propertySeperator ?? new[] { ',' };
            this.ValueSeperator = valueSeperator ?? new[] { '=' };
        }

        public string CommentChars { get; set; }

        public string PropertyStartChar { get; set; }

        public string PropertyEndChar { get; set; }

        public char[] PropertySeperator { get; set; }

        public char[] ValueSeperator { get; set; }
    }
}
