using System;
using Microsoft.Xrm.Sdk;
using ThinkCrm.Core.Interfaces;

namespace ThinkCrm.Core.PluginCore.Attributes
{
    /// <summary>
    /// When present the validation engine will not run. If this attribute is present you must override <see cref="CorePlugin.ValidateExecution"/> if you want validation and may not rely on the base implementation.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class SkipPluginRegistrationValidationAttribute : Attribute, IPluginValidator
    {
        public bool Validate(IPluginExecutionContext context, out bool throwException, out string errorMessage)
        {
            errorMessage = string.Empty;
            throwException = false;

            return true;
        }
    }
}
