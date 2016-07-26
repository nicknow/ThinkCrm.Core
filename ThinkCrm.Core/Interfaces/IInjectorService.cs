namespace ThinkCrm.Core.Interfaces
{
    public interface IInjectorService
    {
        void RegisterType<T,TY>() where TY : T, new();
        void RegisterType<T>(T instance);
        T GetObject<T>() where T : class;
        bool Contains<T>();
    }
}