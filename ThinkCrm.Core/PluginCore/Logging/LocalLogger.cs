using System;
using System.Runtime.CompilerServices;
using ThinkCrm.Core.Interfaces;

namespace ThinkCrm.Core.PluginCore.Logging
{
    public class LocalLogger : ILogging
    {
        private readonly ILogging _logger;
        private readonly string _className;
        private readonly string _methodName;

        public LocalLogger(ILogging logger, string className, [CallerMemberName] string methodName = "")
        {
            if (logger == null) throw new ArgumentNullException(nameof(logger));
            if (className == null) throw new ArgumentNullException(nameof(className));
            if (methodName == null) throw new ArgumentNullException(nameof(methodName));
            _logger = logger;
            _className = className;
            _methodName = methodName;
        }

        public ILogging GetBaseLogger()
        {
            return _logger;
        }

        public void Trace(string format, params object[] args)
        {
            _logger.WithCaller(_className, _methodName, true).Trace(format, args);
        }

        public ILogging WithCaller(string currentClass = "", string currentMethod = "", bool useOnce = true)
        {
            return _logger.WithCaller(_className, _methodName, true);
        }

        public ILogging ClearCaller()
        {
            return _logger.ClearCaller();
        }

        public void Write(string message, params object[] args)
        {
            _logger.WithCaller(_className, _methodName, true).Write(message, args);
        }

        public T WriteAndReturn<T>(T returnItem, string message, params object[] args)
        {
           return _logger.WithCaller(_className, _methodName, true).WriteAndReturn<T>(returnItem, message, args);
        }

        public void Write(Exception ex)
        {
            _logger.WithCaller(_className, _methodName, true).Write(ex);
        }

        public void If(bool condition, string message, params object[] args)
        {
            _logger.WithCaller(_className, _methodName, true).If(condition, message, args);
        }

        public void IfNot(bool condition, string message, params object[] args)
        {
            _logger.WithCaller(_className, _methodName, true).IfNot(condition, message, args);
        }

        public void Debug(string message, params object[] args)
        {
            _logger.WithCaller(_className, _methodName, true).Debug(message, args);
        }

        public void DebugIf(bool condition, string message, params object[] args)
        {
            _logger.WithCaller(_className, _methodName, true).DebugIf(condition, message, args);
        }

        public void DebugIfNot(bool condition, string message, params object[] args)
        {
            _logger.WithCaller(_className, _methodName, true).DebugIfNot(condition, message, args);
        }

        public void AddListener(ILoggingListener listener)
        {
            _logger.WithCaller(_className, _methodName, true).AddListener(listener);
        }

        public void ClearListeners()
        {
            _logger.WithCaller(_className, _methodName, true).ClearListeners();
        }
    }
}
