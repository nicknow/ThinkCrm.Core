using System;
using Microsoft.Xrm.Sdk;
using ThinkCrm.Core.Interfaces;

namespace ThinkCrm.Core.PluginCore.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TargetEntityReferenceAttribute : Attribute, IPluginValidator
    {
        private readonly bool _throwException;
        private readonly TargetGenericAttribute<EntityReference> _targetGenericAttribute; 

        public TargetEntityReferenceAttribute(bool throwException = false)
        {
            _throwException = throwException;
            _targetGenericAttribute = new TargetGenericAttribute<EntityReference>();
        }

        public TargetEntityReferenceAttribute(bool throwException = false, params string[] entities)
        {
            _throwException = throwException;
            _targetGenericAttribute = new TargetGenericAttribute<EntityReference>(entities);
        }

        public bool Validate(IPluginExecutionContext context, out bool throwException, out string errorMessage)
        {
            if (!_targetGenericAttribute.Validate(context.InputParameters))
            {
                throwException = _throwException;
                errorMessage = "TargetEntityReferenceAttribute Validation Failed.";
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
