using System.Collections.Generic;
using System.Linq;
using ThinkCrm.Core.Interfaces;

namespace ThinkCrm.Core.PluginCore.Validator
{
    public class ValidationBuilder : IValidationBuilder
    {   

        private readonly List<IPluginValidator> _validatiors = new List<IPluginValidator>(); 

        public void Add(IPluginValidator validator)
        {
            _validatiors.Add(validator);
        }

        internal List<IPluginValidator> GetAll => _validatiors.ToList();

    }
}
