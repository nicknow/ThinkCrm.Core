using System;
using Microsoft.Xrm.Sdk;
using ThinkCrm.Core.Interfaces;

namespace ThinkCrm.Core.PluginCore.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TargetEntityAttribute : Attribute, IPluginValidator
    {
        private readonly bool _throwException;
        private readonly TargetGenericAttribute<Entity> _targetGenericAttribute;

        public TargetEntityAttribute(bool throwException = true)
        {
            _throwException = throwException;
            _targetGenericAttribute = new TargetGenericAttribute<Entity>();
        }

        public TargetEntityAttribute(bool throwException = true, params string[] entities)
        {
            _throwException = throwException;
            _targetGenericAttribute = new TargetGenericAttribute<Entity>(entities);
        }

        public bool Validate(IPluginExecutionContext context, out bool throwException, out string errorMessage)
        {
            if (!_targetGenericAttribute.Validate(context.InputParameters))
            {
                throwException = _throwException;
                errorMessage = "TargetEntityAttribute Validation Failed.";
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
