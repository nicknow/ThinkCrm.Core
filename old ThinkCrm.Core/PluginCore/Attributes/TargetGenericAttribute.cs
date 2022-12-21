using System;
using System.Linq;
using Microsoft.Xrm.Sdk;
using ThinkCrm.Core.PluginCore.Helper;

namespace ThinkCrm.Core.PluginCore.Attributes
{
    public class TargetGenericAttribute<T>
    {
        private readonly bool _anyEntity;
        private readonly string[] _entities;

        public TargetGenericAttribute()
        {
            _anyEntity = true;
        }

        public TargetGenericAttribute(params string[] entities)
        {
            _anyEntity = false;
            _entities = entities;
        }

        public bool Validate(ParameterCollection targetCollection)
        {
            return targetCollection.Contains(PluginConstants.Target) && targetCollection[PluginConstants.Target] is T &&
                   (_anyEntity ||
                    _entities.Any(
                        x =>
                            x.Equals(ResolveLogicalName(targetCollection[PluginConstants.Target]),
                                StringComparison.InvariantCultureIgnoreCase)));
        }

        private string ResolveLogicalName(object target)
        {
            if (target == null) return string.Empty;
            if (target is Entity) return ((Entity) target).LogicalName;
            if (target is EntityReference) return ((EntityReference) target).LogicalName;

            return string.Empty;
        }
    }
}
