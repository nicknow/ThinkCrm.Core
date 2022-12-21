using System;
using Microsoft.Xrm.Sdk;
using ThinkCrm.Core.Interfaces;

namespace ThinkCrm.Core.PluginCore.Attributes
{
    public class InputParameterCheckAttribute : Attribute, IPluginValidator
    {
        private readonly bool _throwException;
        private readonly string _name;
        private readonly Type _type;

        /// <summary>
        /// Ensures that <see cref="IPluginExecutionContext.InputParameters"/> contains an object with a certain key and object type.
        /// </summary>
        /// <param name="name">Key in collection</param>
        /// <param name="type">Type that collection's value matching key must match</param>
        /// <param name="throwException">Set to true to throw exception or false to end gracefully</param>
        public InputParameterCheckAttribute(string name, Type type, bool throwException = true)
        {
            _throwException = throwException;
            _name = name ?? throw new ArgumentNullException(nameof(name));
            _type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public bool Validate(IPluginExecutionContext context, out bool throwException, out string errorMessage)
        {
            if (context.InputParameters.Contains(_name) && context.InputParameters[_name].GetType() == _type)
            {
                throwException = false;
                errorMessage = string.Empty;
                return true;
            }
            else
            {
                throwException = _throwException;
                errorMessage = $"Validation Failed on InputParameter Contains={_name} and Type={_type}";
                return false;
            }
        }
    }
}
