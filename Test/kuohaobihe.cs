using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Test
{
   public class Kuohaobihe
    {

        public static List<ConditionError> Check(string str)
        {
            char[] ch = str.ToCharArray();
            var lkhNo = 0;
            var rkhNo = 0;
            var st = new Stack<ConditionErrorInfo>();
            var result = new List<ConditionError>();
            var subStr = new List<string>();
            for (int i = 0; i < ch.Length; i++)
            {
                if (ch[i] == '(')
                {
                    lkhNo++;
                    st.Push(new ConditionErrorInfo { Index = i + 1, Type = 1, KhNo = lkhNo });
                }

                if (ch[i] == ')')
                {
                    rkhNo++;
                    if (st.Count == 0)
                    {
                        result.Add(new ConditionError { ErrorInfo = new ConditionErrorInfo { Index = i + 1, Type = 2, KhNo = rkhNo }, Msg = string.Format("第{0}个右括号没有对应的左括号.", rkhNo) });
                        continue;
                    }

                    var v = st.Pop();
                    var subObj = str.Substring(v.Index, i - v.Index);
                    
                    var subLen = subStr.Count;
                    if (subLen > 0)
                    {
                        var preStr = subStr[subLen - 1];
                        if (subObj.Trim().Substring(1,subObj.Length-2).Trim() == preStr.Trim())
                        {
                            result.Add(new ConditionError { ErrorInfo = new ConditionErrorInfo { Index = i + 1, Type = 3, KhNo = rkhNo }, Msg = string.Format("第{0}个左括号与第{1}右括号多余", v.KhNo, rkhNo) });
                        }
                    }

                    subStr.Add(subObj);
                    if (string.IsNullOrEmpty(subObj.Trim()))
                    {
                        result.Add(new ConditionError { ErrorInfo = new ConditionErrorInfo { Index = i + 1, Type = 3, KhNo = rkhNo }, Msg = "条件不能为空." });
                    }
                }
            }

            //if (st.Count == 0 && result.Count<1)
            //{
            //    return null;
            //}

            var n = st.Count;
            for (var i = 0; i < n; i++)
            {
                var last = st.Pop();
                result.Add(new ConditionError { ErrorInfo = last, Msg = string.Format("第{0}个左括号没有对应的右括号.", lkhNo) });
            }

            return result;
        }

        public class ConditionErrorInfo
        {
            // 1 左括号 2 右括号
            public int Type { get; set; }
            // 括号在表达式中的位置
            public int Index { get; set; }

            public int KhNo { get; set; }

        }

        public class ConditionError
        {
            public ConditionErrorInfo ErrorInfo { get; set; }
            public string Msg { get; set; }
        }
      
    }
}
