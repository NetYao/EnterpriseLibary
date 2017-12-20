using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;

namespace Enterprises.Framework.AOP2
{
    /// <summary>
    /// IContextProperty接口仅仅是为Context提供一些基本信息，它并不能完成对方法调用消息的截取。根据对代理技术的分析，要 实现AOP，必须在方法调用截取消息传递，并形成一个消息链Message Sink。因此，如果需要向所在的Context的Transparent Proxy/Real Proxy中植入Message Sink，Context Property还需要提供Sink的功能
    /// </summary>
    public interface IContextProperty
    {
        /// <summary>
        /// 表示Context Property的名字，Name属性值要求在整个Context中必须是唯一的
        /// </summary>
        string Name { get; }
        /// <summary>
        /// IsNewContextOK()方法用于确认Context是否存在 冲突的情况
        /// </summary>
        /// <param name="newCtx"></param>
        /// <returns></returns>
        bool IsNewContextOK(Context newCtx);

        /// <summary>
        /// 通知Context Property，当新的Context构造完成时，则进入Freeze状态（通常情况下，Freeze方法仅提供一个空的实现）
        /// </summary>
        /// <param name="newCtx"></param>
        void Freeze(Context newCtx);
    }
}
