using System;
using System.Text;
using ThinkCrm.Core.Interfaces;

namespace ThinkCrm.Core.PluginCore.Logging
{
    public class LogToStringBuilder : ILoggingListener
    {
        private readonly StringBuilder _stringBuilder;

        public LogToStringBuilder(StringBuilder stringBuilder)
        {
            if (stringBuilder == null) throw new ArgumentNullException(nameof(stringBuilder));
            _stringBuilder = stringBuilder;
        }

        public void Write(string message, string messageUnformatted, params object[] args)
        {
            _stringBuilder?.AppendLine(message);
        }
    }
}
