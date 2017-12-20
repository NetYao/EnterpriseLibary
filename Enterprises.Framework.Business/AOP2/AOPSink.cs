using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace Enterprises.Framework.AOP2
{
    public class Aspect : IMessageSink
    {

        private IMessageSink m_NextSink;

        public Aspect(IMessageSink nextSink)
        {
            m_NextSink = nextSink;
        }

        public IMessageCtrl AsyncProcessMessage(IMessage msg, IMessageSink replySink)
        {
            return null;
        }

        public IMessageSink NextSink
        {
            get { return m_NextSink; }
        }

        private SortedList m_BeforeAdvices;
        private SortedList m_AfterAdvices;

        protected virtual void AddBeforeAdvice(string methodName, IBeforeAdvice before)
        {
            lock (this.m_BeforeAdvices)
            {
                if (!m_BeforeAdvices.Contains(methodName))
                {
                    m_BeforeAdvices.Add(methodName, before);
                }
            }
        }

        protected virtual void AddAfterAdvice(string methodName, IAfterAdvice after)
        {
            lock (this.m_AfterAdvices)
            {
                if (!m_AfterAdvices.Contains(methodName))
                {
                    m_AfterAdvices.Add(methodName, after);
                }
            }
        }

        public IMessage SyncProcessMessage(IMessage msg)
        {
            IMethodCallMessage call = msg as IMethodCallMessage;
            string methodName = call.MethodName.ToUpper();
            IBeforeAdvice before = FindBeforeAdvice(methodName);
            if (before != null)
            {
                before.BeforeAdvice(call);
            }
            IMessage retMsg = m_NextSink.SyncProcessMessage(msg);
            IMethodReturnMessage reply = retMsg as IMethodReturnMessage;
            IAfterAdvice after = FindAfterAdvice(methodName);
            if (after != null)
            {
                after.AfterAdvice(reply);
            }
            return retMsg;
        }


        public IBeforeAdvice FindBeforeAdvice(string methodName)
        {
            IBeforeAdvice before;
            lock (this.m_BeforeAdvices)
            {
                before = (IBeforeAdvice)m_BeforeAdvices[methodName];
            }
            return before;
        }

        public IAfterAdvice FindAfterAdvice(string methodName)
        {
            IAfterAdvice after;
            lock (this.m_AfterAdvices)
            {
                after = (IAfterAdvice)m_AfterAdvices[methodName];
            }
            return after;
        }

        public void ReadAspect(string aspectXml, string aspectName)
        {
            //IBeforeAdvice before = (IBeforeAdvice)Configuration.GetAdvice(aspectXml, aspectName, Advice.Before);
            //string[] methodNames = Configuration.GetNames(aspectXml, aspectName, Advice.Before);
            //foreach (string name in methodNames)
            //{
            //    AddBeforeAdvice(name, before);
            //}
            //IAfterAdvice after = (IAfterAdvice)Configuration.GetAdvice(aspectXml, aspectName, Advice.After);
            //string[] methodNames = Configuration.GetNames(aspectXml, aspectName, Advice.After);
            //foreach (string name in methodNames)
            //{
            //    AddAfterAdvice(name, after);
            //}
        }
    }

    /// <summary>
    /// 通知
    /// </summary>
    public enum Advice
    {
        /// <summary>
        /// 前
        /// </summary>
        Before = 1,
        /// <summary>
        /// 后
        /// </summary>
        After = 2
    }
}


