using System;
using System.Collections.Generic;
using ThinkCrm.Core.Interfaces;

namespace ThinkCrm.Core.Injector
{
    public class InjectorService : IInjectorService
    {
        //Make Tuple bool value True if Object should be created new on each request
        private readonly Dictionary<Type, Tuple<bool, object>> _objectDictionary = new Dictionary<Type, Tuple<bool,object>>();

        public InjectorService(params IInjectable[] injectables)
        {
            if (injectables != null && injectables.Length > 0)
            {
                foreach (var injectable in injectables)
                {
                    injectable.Register(this);
                }
            }
        }

        /// <summary>
        /// Register a type that will be instantiated using a new() constructor each time it is requested.
        /// </summary>
        /// <typeparam name="T">This is the type (in normal usage this will be an interface)</typeparam>
        /// <typeparam name="TY">This is a type which implements <typeparam name="T">T</typeparam> and has a default constructor</typeparam>
        public void RegisterType<T,TY>() where T : class where TY : T, new()
        {
            if (_objectDictionary.ContainsKey(typeof(T))) throw new ArgumentException($"Key already exists in Object Dictionary: {typeof(T)}.");

            _objectDictionary.Add(typeof(T), new Tuple<bool, object>(true,typeof(TY)));
        }

        /// <summary>
        /// Register a type and an instance of that type. The same instance will be returned each time it is requested.
        /// </summary>
        /// <typeparam name="T">This is the type (in normal usage it will be an interface)</typeparam>
        /// <param name="instance">This is the instance of <typeparam name="T">T</typeparam> which will be returned when T is requested</param>
        public void RegisterType<T>(T instance) where T : class 
        {
            if (instance == null) throw new ArgumentNullException(nameof(instance));
            if (_objectDictionary.ContainsKey(typeof(T))) throw new ArgumentException($"Key already exists in Object Dictionary: {typeof(T)}.");

            _objectDictionary.Add(typeof(T), new Tuple<bool, object>(false,instance));
        }

        /// <summary>
        /// Returns an instance of the requested type
        /// </summary>
        /// <typeparam name="T">This is the type (in normal usage it will be an interface) to be returned</typeparam>
        /// <returns></returns>
        public T GetObject<T>() where T : class 
        {
            if (!_objectDictionary.ContainsKey(typeof(T))) throw new KeyNotFoundException($"Key Not Found in Object Dictionary: {typeof(T)}.");

            if (_objectDictionary[typeof(T)].Item1)
                return Activator.CreateInstance((Type) _objectDictionary[typeof(T)].Item2) as T;

            return _objectDictionary[typeof(T)].Item2 as T;
        }

        /// <summary>
        /// Returns true if the specificed type (in normal usage it will be an interface) has been registered
        /// </summary>
        /// <typeparam name="T">This is the type (in normal usage it will be an interface) to be returned</typeparam>
        /// <returns>True if the type has been registered.</returns>
        public bool Contains<T>()
        {
            return _objectDictionary.ContainsKey(typeof(T));
        }

    }
}
