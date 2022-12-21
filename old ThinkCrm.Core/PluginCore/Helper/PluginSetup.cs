using System;
using System.Collections.Generic;
using Microsoft.Xrm.Sdk;
using ThinkCrm.Core.Interfaces;
using ThinkCrm.Core.PluginCore.Logging;

namespace ThinkCrm.Core.PluginCore.Helper
{
    public class PluginSetup : IPluginSetup
    {
        private IPluginExecutionContext _context;
        private readonly IServiceProvider _serviceProvider;
        private IOrganizationServiceFactory _serviceFactory;
        private ILogging _logging;
        private ITracingService _tracing;
        private readonly Dictionary<Guid, ICrmService> _serviceDictionary;
        private IPluginSetupHelper _helper;

        private readonly string _pluginName;
        private readonly string _unsecureConfiguration;
        private readonly string _secureConfiguration;

        public PluginSetup(IServiceProvider serviceProvider, string pluginName = "", string unsecureConfiguration = "", string secureConfiguration = "")
        {
            if (serviceProvider == null) throw new Exception("PluginSetup requires a valid IServiceProvider.");

            _serviceProvider = serviceProvider;
            _pluginName = pluginName;
            _serviceDictionary = new Dictionary<Guid, ICrmService>();
            _secureConfiguration = secureConfiguration;
            _unsecureConfiguration = unsecureConfiguration;
        }

        public IServiceProvider ServiceProvider => _serviceProvider;

        public IPluginExecutionContext Context
        {
            get
            {
                if (_context != null)
                {
                    return _context;
                }
                else
                {
                    _context =
                        (Microsoft.Xrm.Sdk.IPluginExecutionContext)
                            ServiceProvider.GetService(typeof (Microsoft.Xrm.Sdk.IPluginExecutionContext));
                    return _context;
                }
            }
        }

        public IOrganizationServiceFactory ServiceFactory
        {
            get
            {
                if (_serviceFactory != null)
                {
                    return _serviceFactory;
                }
                else
                {
                    _serviceFactory =
                        (IOrganizationServiceFactory) ServiceProvider.GetService(typeof (IOrganizationServiceFactory));
                    return _serviceFactory;
                }
            }
        }

        public IPluginSetupHelper Helper => _helper ?? (_helper = new PluginSetupHelper(this));

        public ICrmService GetService(Guid? userId)
        {
            if (!_serviceDictionary.ContainsKey(userId ?? Guid.Empty))
            {
                _serviceDictionary.Add(userId ?? Guid.Empty, InternalNewCrmService(userId));
            }

            return _serviceDictionary[userId ?? Guid.Empty];
        }

        public string UnsecureConfiguration => _unsecureConfiguration;

        public string SecureConfiguration => _secureConfiguration;

        private IOrganizationService InternalNewService(Guid? userId) => ServiceFactory.CreateOrganizationService(userId);

        private ICrmService InternalNewCrmService(Guid? userId) => new CrmService.CrmService(InternalNewService(userId),Logging);

        public ICrmService UserService => this.GetService(Context.UserId);

        public ICrmService InitiatingUserService => this.GetService(Context.InitiatingUserId);

        public ICrmService SystemService => this.GetService(null);

        public ILogging Logging
        {
            get
            {
                if (_logging != null)
                {
                    return _logging;
                }
                else
                {
                    _tracing = (ITracingService) ServiceProvider.GetService(typeof (ITracingService));
                    _logging = new Logger(_pluginName, Context.GetIsolationMode() == IsolationMode.Sandbox);
                    _logging.AddListener(new LogToCrmTracingService(_tracing));
                    return _logging;
                }
            }
        }
    }
}