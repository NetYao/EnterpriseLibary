namespace Enterprises.Framework.Plugin.Office.Converters.Model
{
    /// <summary>
    /// 书签数据对象
    /// </summary>
    public class DataItem
    {
        /// <summary>
        /// 
        /// </summary>
        public DataItem() { }

        internal DataItem(RenderMode mode, object value)
        {
            Value = value;
            Type = mode;
        }

        public static DataItem NewItem(RenderMode mode, object value)
        {
            return new DataItem(mode, value);
        }

        /// <summary>
        /// 书签类型
        /// </summary>
        public RenderMode Type
        {
            get;
            set;
        }
        
        /// <summary>
        /// 书签值
        /// </summary>
        public object Value
        {
            get;
            set;
        }
    }
}
