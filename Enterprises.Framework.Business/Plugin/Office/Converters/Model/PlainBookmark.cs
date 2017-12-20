namespace Enterprises.Framework.Plugin.Office.Converters.Model
{
    /// <summary>
    /// 文本书签
    /// </summary>
    public class PlainBookmark : Bookmark
    {
        public override void Instantiate(object data)
        {
            string text = data.ToString();
            Range.Text = text;
        }

        /// <summary>
        /// 
        /// </summary>
        public PlainBookmark() { }

        protected override object CloneInternal()
        {
            return new PlainBookmark();
        } 
    }
}
