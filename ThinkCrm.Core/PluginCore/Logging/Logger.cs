using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using ThinkCrm.Core.Interfaces;

namespace ThinkCrm.Core.PluginCore.Logging
{
    public class Logger : ILogging
    {
        private readonly string _prependText;
        private readonly bool _isInSandbox;
        private readonly List<ILoggingListener> _listeners;

        public Logger(string prependText = "", bool isInSandbox = true, ILoggingListener listener = null)
        {
            _prependText = prependText;
            _isInSandbox = isInSandbox;
            _listeners = new List<ILoggingListener>();
            if (listener != null) _listeners.Add(listener);
        }

        private string _currentClass = string.Empty;
        private string _currentMethod = string.Empty;
        private bool _prependData = false;
        private bool _useOnce = true;

        public ILogging WithCaller(string currentClass = "", [CallerMemberName] string currentMethod = "",
            bool useOnce = true)
        {
            _currentClass = currentClass;
            _currentMethod = currentMethod;
            _prependData = true;
            _useOnce = useOnce;

            return this;
        }

        public ILogging ClearCaller()
        {
            _prependData = false;
            _useOnce = false;
            _currentClass = string.Empty;
            _currentMethod = string.Empty;

            return this;
        }

        public void Write(string message, params object[] args)
        {

            var compiledMessage =
                $"{(string.IsNullOrEmpty(_prependText) ? string.Empty : _prependText)}{(string.IsNullOrEmpty(_prependText) ? string.Empty : "|")}{(!_prependData ? string.Empty : $"{_currentClass}|{_currentMethod}")}|{DateTime.Now}|{(!args.Any() ? message : string.Format(message, args))}";

            _listeners.ForEach(t => t.Write(compiledMessage, message, args));

            if (_prependData && _useOnce) ClearCaller();

        }

        public void Trace(string format, params object[] args)
        {
            this.Write(format, args);
        }

        public T WriteAndReturn<T>(T returnItem, string message, params object[] args)
        {
            this.Write(message, args);
            return returnItem;
        }

        public void Write(Exception ex)
        {
            this.Write(_isInSandbox
                ? SandboxedExceptionHandling.GetExtendedExceptionDetails(ex)
                : NonSandboxedExceptionLogging.GetExtendedExceptionDetails(ex));
        }

        public void If(bool condition, string message, params object[] args)
        {
            if (condition) this.Write(message, args);
        }

        public void IfNot(bool condition, string Message, params object[] args)
        {
            if (!condition) this.Write(Message, args);
        }

        public void Debug(string message, params object[] args)
        {
#if DEBUG
            this.Write(message, args);
#else
            return;            
#endif
        }

        public void DebugIf(bool condition, string message, params object[] args)
        {
#if DEBUG
            this.If(condition, message, args);
#else
            return;            
#endif
        }

        public void DebugIfNot(bool condition, string message, params object[] args)
        {
#if DEBUG
            this.IfNot(condition, message, args);
#else
            return;            
#endif
        }

        public void AddListener(ILoggingListener listener)
        {
            if (!_listeners.Contains(listener)) _listeners.Add(listener);
        }

        public void ClearListeners()
        {
            _listeners.Clear();
        }


    }
}
