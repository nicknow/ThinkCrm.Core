using Microsoft.Xrm.Sdk;

namespace ThinkCrm.Core.Interfaces
{
    public interface IPluginSetupHelper
    {
        Entity GetTargetEntity();
        T GetTargetEntity<T>() where T: Entity;
        EntityReference GetTargetReference();
    }
}