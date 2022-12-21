using System;
using ThinkCrm.Core.Interfaces;


namespace ThinkCrm.Core.CrmService
{ 
    public class NullLogger : ILogging
    {
        public void Trace(string format, params object[] args)
        {
            return;
        }

        public ILogging WithCaller(string currentClass = "", string currentMethod = "", bool useOnce = true)
        {
            return this;
        }

        public ILogging ClearCaller()
        {
            return this;
        }

        public void Write(string message, params object[] args)
        {
            return;
        }

        public T WriteAndReturn<T>(T returnItem, string message, params object[] args)
        {
            return returnItem;
        }

        public void Write(Exception ex)
        {
            return;
        }

        public void If(bool condition, string message, params object[] args)
        {
            return;
        }

        public void IfNot(bool condition, string message, params object[] args)
        {
            return;
        }

        public void Debug(string message, params object[] args)
        {
            return;
        }

        public void DebugIf(bool condition, string message, params object[] args)
        {
            return;
        }

        public void DebugIfNot(bool condition, string message, params object[] args)
        {
            return;
        }

        public void AddListener(ILoggingListener listener)
        {
            return;
        }

        public void ClearListeners()
        {
            return;
        }
    }
}
