using Microsoft.Xrm.Sdk;

namespace ThinkCrm.Core.Interfaces
{
    public interface IPluginValidator
    {
        /// <summary>
        /// Checks the passed IPluginExecutionContext against the Attribute configuration to determine if validation is successful.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="throwException">True if an Exception should be thrown. If false and Validate is false plugin execution should be terminated without error.</param>
        /// <param name="errorMessage"></param>
        /// <returns>True if validation passes, False if validation fails</returns>
        bool Validate(IPluginExecutionContext context, out bool throwException, out string errorMessage);
    }
}
