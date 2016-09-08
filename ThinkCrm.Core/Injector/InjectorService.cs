using System;
using System.Collections.Generic;
using ThinkCrm.Core.Interfaces;

namespace ThinkCrm.Core.Injector
{
    public class InjectorService : IInjectorService
    {
        //Make Tuple bool value True if Object should be created new on each request
        private readonly Dictionary<Type, Tuple<bool, object>> _objectDictionary = new Dictionary<Type, Tuple<bool,object>>();        

        public void RegisterType<T,TY>() where T : class where TY : T, new()
        {
            if (_objectDictionary.ContainsKey(typeof(T))) throw new ArgumentException($"Key already exists in Object Dictionary: {typeof(T)}.");

            _objectDictionary.Add(typeof(T), new Tuple<bool, object>(true,typeof(TY)));
        }

        public void RegisterType<T>(T instance) where T : class 
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            if (_objectDictionary.ContainsKey(typeof(T))) throw new ArgumentException($"Key already exists in Object Dictionary: {typeof(T)}.");

            _objectDictionary.Add(typeof(T), new Tuple<bool, object>(false,instance));
        }

        public T GetObject<T>() where T : class 
        {
            if (!_objectDictionary.ContainsKey(typeof(T))) throw new KeyNotFoundException($"Key Not Found in Object Dictionary: {typeof(T)}.");

            if (_objectDictionary[typeof(T)].Item1)
                return Activator.CreateInstance((Type) _objectDictionary[typeof(T)].Item2) as T;

            return _objectDictionary[typeof(T)].Item2 as T;
        }

        public bool Contains<T>()
        {
            return _objectDictionary.ContainsKey(typeof(T));
        }

    }
}
