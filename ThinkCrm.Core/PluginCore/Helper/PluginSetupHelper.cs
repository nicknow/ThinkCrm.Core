using System;
using Microsoft.Xrm.Sdk;
using ThinkCrm.Core.Interfaces;

namespace ThinkCrm.Core.PluginCore.Helper
{
    public class PluginSetupHelper : IPluginSetupHelper
    {
        private readonly IPluginSetup _pluginSetup;
        private readonly ILogging _logging;
        private readonly string _className;

        internal PluginSetupHelper(IPluginSetup pluginSetup)
        {
            _pluginSetup = pluginSetup;
            _logging = _pluginSetup.Logging;
            _className = GetType().FullName;
        }

        public Entity GetTargetEntity()
        {
            if (_pluginSetup.Context.InputParameters.Contains(PluginConstants.Target) &&
                _pluginSetup.Context.InputParameters[PluginConstants.Target] is Entity)
            {
                return (Entity)_pluginSetup.Context.InputParameters[PluginConstants.Target];
            }
            _logging.WithCaller(_className).Write(
                "Error: InputParameters does not contain a Target or Target is not Entity. Contains: {0} / Type: {1}",
                _pluginSetup.Context.InputParameters.Contains(PluginConstants.Target),
                _pluginSetup.Context.InputParameters.Contains(PluginConstants.Target)
                    ? _pluginSetup.Context.InputParameters[PluginConstants.Target].GetType().ToString()
                    : "(Not Applicable)");
            throw new InvalidPluginExecutionException(PluginConstants.UserErrorMessage);
        }

        public T GetTargetEntity<T>() where T: Entity
        {
            try
            {
                return this.GetTargetEntity().ToEntity<T>();
            }
            catch (InvalidPluginExecutionException ex)
            {
                _logging.WithCaller(_className).Write(ex);
                throw;
            }
            catch (Exception ex)
            {
                _logging.WithCaller(_className).Write(ex);
                throw new InvalidPluginExecutionException(PluginConstants.UserErrorMessage, ex);
            }
        }

        public EntityReference GetTargetReference()
        {
            if (_pluginSetup.Context.InputParameters.Contains(PluginConstants.Target) &&
                _pluginSetup.Context.InputParameters[PluginConstants.Target] is EntityReference)
            {
                return (EntityReference)_pluginSetup.Context.InputParameters[PluginConstants.Target];
            }
            _logging.WithCaller(_className).Write(
                "Error: InputParameters does not contain a Target or Target is not EntityReference. Contains: {0} / Type: {1}",
                _pluginSetup.Context.InputParameters.Contains(PluginConstants.Target),
                _pluginSetup.Context.InputParameters.Contains(PluginConstants.Target)
                    ? _pluginSetup.Context.InputParameters[PluginConstants.Target].GetType().ToString()
                    : "(Not Applicable)");
            throw new InvalidPluginExecutionException(PluginConstants.UserErrorMessage);
        }
    }
}
