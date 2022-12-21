using ThinkCrm.Core.Interfaces;

namespace ThinkCrm.Core
{
    public class InjectableObject<T> : IInjectable where T : class
    {
        private readonly T _instance;

        public InjectableObject(T instance)
        {
            _instance = instance;
        }

        public void Register(IInjectorService injectorService)
        {
            injectorService.RegisterType<T>(_instance);
        }
    }
}