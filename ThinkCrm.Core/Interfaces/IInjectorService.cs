namespace ThinkCrm.Core.Interfaces
{
    public interface IInjectorService
    {
        void RegisterType<T, TY>() where T : class where TY : T, new();
        void RegisterType<T>(T instance) where T : class;
        T GetObject<T>() where T : class;
        bool Contains<T>();
    }
}