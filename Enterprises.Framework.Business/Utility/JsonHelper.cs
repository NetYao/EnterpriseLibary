namespace Enterprises.Framework.Utility
{
    /// <summary>
    /// 分隔Json字符串为字典集合。
    /// </summary>
    public class JsonHelper
    {
        /// <summary>
        /// 是否Json字符串字符串开始
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static bool IsJsonStart(ref string json)
        {
            if (!string.IsNullOrEmpty(json))
            {
                json = json.Trim('\r', '\n', ' ');
                if (json.Length > 1)
                {
                    char s = json[0];
                    char e = json[json.Length - 1];
                    return (s == '{' && e == '}') || (s == '[' && e == ']');
                }
            }
            return false;
        }

        /// <summary>
        /// 判断是否json字符串
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static bool IsJson(string json)
        {
            int errIndex;
            return IsJson(json, out errIndex);
        }

        /// <summary>
        /// 判断是否json字符串
        /// </summary>
        /// <param name="json"></param>
        /// <param name="errIndex">错误位置</param>
        /// <returns></returns>
        public static bool IsJson(string json, out int errIndex)
        {
            errIndex = 0;
            if (IsJsonStart(ref json))
            {
                var cs = new CharState();
                for (int i = 0; i < json.Length; i++)
                {
                    char c = json[i];
                    if (SetCharState(c, ref cs) && cs.ChildrenStart) //设置关键符号状态。
                    {
                        string item = json.Substring(i);
                        int err;
                        int length = GetValueLength(item, true, out err);
                        cs.ChildrenStart = false;
                        if (err > 0)
                        {
                            errIndex = i + err;
                            return false;
                        }
                        i = i + length - 1;
                    }
                    if (cs.IsError)
                    {
                        errIndex = i;
                        return false;
                    }
                }

                return !cs.ArrayStart && !cs.JsonStart;
            }
            return false;
        }

        /// <summary>
        /// 获取值的长度（当Json值嵌套以"{"或"["开头时）
        /// </summary>
        private static int GetValueLength(string json, bool breakOnErr, out int errIndex)
        {
            errIndex = 0;
            int len = 0;
            if (!string.IsNullOrEmpty(json))
            {
                var cs = new CharState();
                for (int i = 0; i < json.Length; i++)
                {
                    char c = json[i];
                    if (!SetCharState(c, ref cs)) //设置关键符号状态。
                    {
                        if (!cs.JsonStart && !cs.ArrayStart) //json结束，又不是数组，则退出。
                        {
                            break;
                        }
                    }
                    else if (cs.ChildrenStart) //正常字符，值状态下。
                    {
                        int length = GetValueLength(json.Substring(i), breakOnErr, out errIndex); //递归子值，返回一个长度。。。
                        cs.ChildrenStart = false;
                        cs.ValueStart = 0;
                        //cs.state = 0;
                        i = i + length - 1;
                    }
                    if (breakOnErr && cs.IsError)
                    {
                        errIndex = i;
                        return i;
                    }
                    if (!cs.JsonStart && !cs.ArrayStart) //记录当前结束位置。
                    {
                        len = i + 1; //长度比索引+1
                        break;
                    }
                }
            }
            return len;
        }

        /// <summary>
        /// 字符状态
        /// </summary>
        private class CharState
        {
            internal bool JsonStart; //以 "{"开始了...
            internal bool SetDicValue; // 可以设置字典值了。
            internal bool EscapeChar; //以"\"转义符号开始了

            /// <summary>
            /// 数组开始【仅第一开头才算】，值嵌套的以【childrenStart】来标识。
            /// </summary>
            internal bool ArrayStart; //以"[" 符号开始了

            internal bool ChildrenStart; //子级嵌套开始了。

            /// <summary>
            /// 【0 初始状态，或 遇到“,”逗号】；【1 遇到“：”冒号】
            /// </summary>
            internal int State;

            /// <summary>
            /// 【-1 取值结束】【0 未开始】【1 无引号开始】【2 单引号开始】【3 双引号开始】
            /// </summary>
            internal int KeyStart;

            /// <summary>
            /// 【-1 取值结束】【0 未开始】【1 无引号开始】【2 单引号开始】【3 双引号开始】
            /// </summary>
            internal int ValueStart;

            internal bool IsError; //是否语法错误。

            internal void CheckIsError(char c) //只当成一级处理（因为GetLength会递归到每一个子项处理）
            {
                if (KeyStart > 1 || ValueStart > 1)
                {
                    return;
                }
                //示例 ["aa",{"bbbb":123,"fff","ddd"}] 
                switch (c)
                {
                    case '{': //[{ "[{A}]":[{"[{B}]":3,"m":"C"}]}]
                        IsError = JsonStart && State == 0; //重复开始错误 同时不是值处理。
                        break;
                    case '}':
                        IsError = !JsonStart || (KeyStart != 0 && State == 0); //重复结束错误 或者 提前结束{"aa"}。正常的有{}
                        break;
                    case '[':
                        IsError = ArrayStart && State == 0; //重复开始错误
                        break;
                    case ']':
                        IsError = !ArrayStart || JsonStart; //重复开始错误 或者 Json 未结束
                        break;
                    case '"':
                    case '\'':
                        IsError = !(JsonStart || ArrayStart); //json 或数组开始。
                        if (!IsError)
                        {
                            //重复开始 [""",{"" "}]
                            IsError = (State == 0 && KeyStart == -1) || (State == 1 && ValueStart == -1);
                        }
                        if (!IsError && ArrayStart && !JsonStart && c == '\'') //['aa',{}]
                        {
                            IsError = true;
                        }
                        break;
                    case ':':
                        IsError = !JsonStart || State == 1; //重复出现。
                        break;
                    case ',':
                        IsError = !(JsonStart || ArrayStart); //json 或数组开始。
                        if (!IsError)
                        {
                            if (JsonStart)
                            {
                                IsError = State == 0 || (State == 1 && ValueStart > 1); //重复出现。
                            }
                            else if (ArrayStart) //["aa,] [,]  [{},{}]
                            {
                                IsError = KeyStart == 0 && !SetDicValue;
                            }
                        }
                        break;
                    case ' ':
                    case '\r':
                    case '\n': //[ "a",\r\n{} ]
                    case '\0':
                    case '\t':
                        break;
                    default: //值开头。。
                        IsError = (!JsonStart && !ArrayStart) || (State == 0 && KeyStart == -1) ||
                                  (ValueStart == -1 && State == 1); //
                        break;
                }
                //if (isError)
                //{

                //}
            }
        }

        /// <summary>
        /// 设置字符状态(返回true则为关键词，返回false则当为普通字符处理）
        /// </summary>
        private static bool SetCharState(char c, ref CharState cs)
        {
            cs.CheckIsError(c);
            switch (c)
            {
                case '{': //[{ "[{A}]":[{"[{B}]":3,"m":"C"}]}]

                    #region 大括号

                    if (cs.KeyStart <= 0 && cs.ValueStart <= 0)
                    {
                        cs.KeyStart = 0;
                        cs.ValueStart = 0;
                        if (cs.JsonStart && cs.State == 1)
                        {
                            cs.ChildrenStart = true;
                        }
                        else
                        {
                            cs.State = 0;
                        }
                        cs.JsonStart = true; //开始。
                        return true;
                    }

                    #endregion

                    break;
                case '}':

                    #region 大括号结束

                    if (cs.KeyStart <= 0 && cs.ValueStart < 2 && cs.JsonStart)
                    {
                        cs.JsonStart = false; //正常结束。
                        cs.State = 0;
                        cs.KeyStart = 0;
                        cs.ValueStart = 0;
                        cs.SetDicValue = true;
                        return true;
                    }
                    // cs.isError = !cs.jsonStart && cs.state == 0;

                    #endregion

                    break;
                case '[':

                    #region 中括号开始

                    if (!cs.JsonStart)
                    {
                        cs.ArrayStart = true;
                        return true;
                    }

                    if (cs.JsonStart && cs.State == 1)
                    {
                        cs.ChildrenStart = true;
                        return true;
                    }

                    #endregion

                    break;
                case ']':

                    #region 中括号结束

                    if (cs.ArrayStart && !cs.JsonStart && cs.KeyStart <= 2 && cs.ValueStart <= 0) //[{},333]//这样结束。
                    {
                        cs.KeyStart = 0;
                        cs.ValueStart = 0;
                        cs.ArrayStart = false;
                        return true;
                    }

                    #endregion

                    break;
                case '"':
                case '\'':

                    #region 引号

                    if (cs.JsonStart || cs.ArrayStart)
                    {
                        if (cs.State == 0) //key阶段,有可能是数组["aa",{}]
                        {
                            if (cs.KeyStart <= 0)
                            {
                                cs.KeyStart = (c == '"' ? 3 : 2);
                                return true;
                            }

                            if ((cs.KeyStart == 2 && c == '\'') || (cs.KeyStart == 3 && c == '"'))
                            {
                                if (!cs.EscapeChar)
                                {
                                    cs.KeyStart = -1;
                                    return true;
                                }

                                cs.EscapeChar = false;
                            }
                        }
                        else if (cs.State == 1 && cs.JsonStart) //值阶段必须是Json开始了。
                        {
                            if (cs.ValueStart <= 0)
                            {
                                cs.ValueStart = (c == '"' ? 3 : 2);
                                return true;
                            }

                            if ((cs.ValueStart == 2 && c == '\'') || (cs.ValueStart == 3 && c == '"'))
                            {
                                if (!cs.EscapeChar)
                                {
                                    cs.ValueStart = -1;
                                    return true;
                                }

                                cs.EscapeChar = false;
                            }
                        }
                    }

                    #endregion

                    break;
                case ':':

                    #region 冒号

                    if (cs.JsonStart && cs.KeyStart < 2 && cs.ValueStart < 2 && cs.State == 0)
                    {
                        if (cs.KeyStart == 1)
                        {
                            cs.KeyStart = -1;
                        }
                        cs.State = 1;
                        return true;
                    }
                    // cs.isError = !cs.jsonStart || (cs.keyStart < 2 && cs.valueStart < 2 && cs.state == 1);

                    #endregion

                    break;
                case ',':

                    #region 逗号 //["aa",{aa:12,}]

                    if (cs.JsonStart)
                    {
                        if (cs.KeyStart < 2 && cs.ValueStart < 2 && cs.State == 1)
                        {
                            cs.State = 0;
                            cs.KeyStart = 0;
                            cs.ValueStart = 0;
                            //if (cs.valueStart == 1)
                            //{
                            //    cs.valueStart = 0;
                            //}
                            cs.SetDicValue = true;
                            return true;
                        }
                    }
                    else if (cs.ArrayStart && cs.KeyStart <= 2)
                    {
                        cs.KeyStart = 0;
                        //if (cs.keyStart == 1)
                        //{
                        //    cs.keyStart = -1;
                        //}
                        return true;
                    }

                    #endregion

                    break;
                case ' ':
                case '\r':
                case '\n': //[ "a",\r\n{} ]
                case '\0':
                case '\t':
                    if (cs.KeyStart <= 0 && cs.ValueStart <= 0) //cs.jsonStart && 
                    {
                        return true; //跳过空格。
                    }
                    break;
                default: //值开头。。
                    if (c == '\\') //转义符号
                    {
                        if (cs.EscapeChar)
                        {
                            cs.EscapeChar = false;
                        }
                        else
                        {
                            cs.EscapeChar = true;
                            return true;
                        }
                    }
                    else
                    {
                        cs.EscapeChar = false;
                    }
                    if (cs.JsonStart || cs.ArrayStart) // Json 或数组开始了。
                    {
                        if (cs.KeyStart <= 0 && cs.State == 0)
                        {
                            cs.KeyStart = 1; //无引号的
                        }
                        else if (cs.ValueStart <= 0 && cs.State == 1 && cs.JsonStart) //只有Json开始才有值。
                        {
                            cs.ValueStart = 1; //无引号的
                        }
                    }
                    break;
            }
            return false;
        }
    }

}


