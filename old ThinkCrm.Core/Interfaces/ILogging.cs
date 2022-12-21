using System;
using Microsoft.Xrm.Sdk;

namespace ThinkCrm.Core.Interfaces
{
    public interface ILogging : ITracingService
    {
        ILogging WithCaller(string currentClass = "", string currentMethod = "", bool useOnce = true);

        ILogging ClearCaller();

        void Write(string message, params object[] args);

        T WriteAndReturn<T>(T returnItem, string message, params object[] args);

        void Write(Exception ex);

        void If(bool condition, string message, params object[] args);

        void IfNot(bool condition, string message, params object[] args);
        
        void Debug(string message, params object[] args);

        void DebugIf(bool condition, string message, params object[] args);

        void DebugIfNot(bool condition, string message, params object[] args);
       
        void AddListener(ILoggingListener listener);

        void ClearListeners();

    }

}
