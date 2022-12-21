using System;
using Microsoft.Xrm.Sdk;
using ThinkCrm.Core.Interfaces;

namespace ThinkCrm.Core.PluginCore.Attributes
{

    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class MaxDepthAttribute : Attribute, IPluginValidator
    {
        private readonly int _maxDepth;
        private readonly bool _throwException;

        /// <summary>
        /// Ensures that <see cref="IPluginExecutionContext.Depth"/> does not exceed a specified value.
        /// </summary>
        /// <param name="maxDepth">When the IPluginExecutionContext.Depth is greater than this number validation will fail.</param>
        /// <param name="throwException">Default is false which causes the plugin to end gracefully. Set to true to result in an Exceeption being raised</param>
        public MaxDepthAttribute(int maxDepth, bool throwException = false)
        {           
            _maxDepth = maxDepth;
            _throwException = throwException;
        }

        public bool Validate(IPluginExecutionContext context, out bool throwException, out string errorMessage)
        {
            if (context.Depth > _maxDepth)
            {
                throwException = _throwException;
                errorMessage = $"MaxDepth Validation Failed. Allowed: {_maxDepth}  Actual={context.Depth}";
                return false;
            }
            else
            {
                throwException = false;
                errorMessage = string.Empty;
                return true;
            }
        }
    }
}
