using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace Enterprises.Framework.AOP2
{
    public abstract class AOPProperty : IContextProperty, IContributeServerContextSink
    {
        private string _mAspectXml;
        public AOPProperty()
        {
            _mAspectXml = string.Empty;
        }

        public string AspectXml
        {
            set { _mAspectXml = value; }
        }
        protected abstract IMessageSink CreateAspect(IMessageSink nextSink);
        protected virtual string GetName()
        {
            return "AOP";
        }
        protected virtual void FreezeImpl(Context newContext)
        {
            return;
        }
        protected virtual bool CheckNewContext(Context newCtx)
        {
            return true;
        }

        #region IContextProperty Members
        public void Freeze(Context newContext)
        {
            FreezeImpl(newContext);
        }
        public bool IsNewContextOK(Context newCtx)
        {
            return CheckNewContext(newCtx);
        }
        public string Name
        {
            get { return GetName(); }
        }
        #endregion

        #region IContributeServerContextSink Members
        public IMessageSink GetServerContextSink(IMessageSink nextSink)
        {
            Aspect aspect = (Aspect)CreateAspect(nextSink);
            aspect.ReadAspect(_mAspectXml, Name);
            return (IMessageSink)aspect;
        }
        #endregion
       
    }
}
