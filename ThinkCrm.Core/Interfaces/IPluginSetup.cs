using System;
using Microsoft.Xrm.Sdk;

namespace ThinkCrm.Core.Interfaces
{
    public interface IPluginSetup
    {
        IServiceProvider ServiceProvider { get; }
        IPluginExecutionContext Context { get; }
        IOrganizationServiceFactory ServiceFactory { get; }
        IPluginSetupHelper Helper { get; }
        ICrmService UserService { get; }
        ICrmService InitiatingUserService { get; }
        ICrmService SystemService { get; }
        ILogging Logging { get; }
        ICrmService GetService(Guid? userId);
        string UnsecureConfiguration { get; }
        string SecureConfiguration { get; }

    }
}
