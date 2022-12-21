using System;
using ThinkCrm.Core.Interfaces;

namespace ThinkCrm.Core
{
    public class InjectableType<T, TY> : IInjectable where T : class where TY : T, new()
    {
        public void Register(IInjectorService injectorService)
        {
            injectorService.RegisterType<T,TY>();
        }
    }
}