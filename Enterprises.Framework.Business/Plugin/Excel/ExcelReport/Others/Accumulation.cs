namespace Enterprises.Framework.Plugin.Excel.ExcelReport
{
    /// <summary>
    /// 累加器
    /// </summary>
    public class Accumulation
    {
        #region 成员字段及属性

        private int _value;

        public int Value
        {
            get { return _value; }
        }

        #endregion

        #region 0 构造函数
        /// <summary>
        /// 实例化累加器
        /// </summary>
        public Accumulation()
        {
            _value = 0;
        }

        #endregion

        #region 1.0 累加增量

        /// <summary>
        ///     累加增量
        /// </summary>
        /// <param name="increment">增量</param>
        public void Add(int increment)
        {
            _value += increment;
        }

        #endregion
    }
}