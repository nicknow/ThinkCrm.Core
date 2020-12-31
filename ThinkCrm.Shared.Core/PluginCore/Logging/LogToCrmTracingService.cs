using System;
using Microsoft.Xrm.Sdk;
using ThinkCrm.Core.Interfaces;

namespace ThinkCrm.Core.PluginCore.Logging
{
    public class LogToCrmTracingService : ILoggingListener
    {
        private readonly ITracingService _tracingService;

        public LogToCrmTracingService(ITracingService tracingService)
        {
            if (tracingService == null) throw new ArgumentNullException("tracingService");
            _tracingService = tracingService;
        }

        public void Write(string message, string messageUnformatted, params object[] args)
        {
            if (_tracingService != null) _tracingService.Trace(message);
        }
    }
}
