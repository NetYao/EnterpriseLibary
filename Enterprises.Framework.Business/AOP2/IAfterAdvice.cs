using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace Enterprises.Framework.AOP2
{
    public interface IBeforeAdvice
    {
        void BeforeAdvice(IMethodCallMessage callMsg);
    }
    public interface IAfterAdvice
    {
        void AfterAdvice(IMethodReturnMessage returnMsg);
    }
}
