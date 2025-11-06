using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace DI
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Method)]
    public sealed class InjectAttribute : Attribute
    {
        public InjectAttribute()
        {

        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public sealed class ProviderAttribute : Attribute
    {
        public ProviderAttribute()
        {
        }
    }

    public class ClassA : MonoBehaviour
    {
        
    }

    public interface IDependencyProvider
    {

    }

    public class Injector : Singleton<Injector>
    {
        const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        readonly Dictionary<Type, object> registry = new Dictionary<Type, object>();

        private void Awake()
        {
            var providers = FindMonoBehaviours().OfType<IDependencyProvider>();

            foreach (var provider in providers)
            {
                RegisterProvider(provider);
            }

            var injectables = FindMonoBehaviours().Where(IsInjectable);

            foreach (var injectable in injectables)
            {
                InjectDependencies(injectable);
            }            
        }

        void InjectDependencies(object instance)
        {
            var type = instance.GetType();
            var injectableFields = type.GetFields(bindingFlags)
                .Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));

            foreach (var injectableField in injectableFields)
            {
                var fieldType = injectableField.FieldType;
                var resolvedInstance = Reslove(fieldType);

                if(resolvedInstance == null)
                    throw new Exception($"No registered instance for type {fieldType.Name}");

                injectableField.SetValue(instance, resolvedInstance);                
            }

            var injectableMethods = type.GetMethods(bindingFlags)
                .Where(member => Attribute.IsDefined(member, typeof(InjectAttribute)));

            foreach (var injectableMethod in injectableMethods)
            {
                var requiredParameters = injectableMethod.GetParameters()
                    .Select(param => param.ParameterType)
                    .ToArray();

                var resolvedInstances = requiredParameters.Select(Reslove).ToArray();

                if (resolvedInstances.Any(resolvedInstance => resolvedInstance == null))
                {
                    throw new Exception($"No registered instance for one of the parameters in method {injectableMethod.Name}");
                }

                injectableMethod.Invoke(instance, resolvedInstances);
            }
        }

        object Reslove(Type type)
        {
            registry.TryGetValue(type, out var reslovedInstance);
            return reslovedInstance;
        }

        static bool IsInjectable(MonoBehaviour obj)
        {
            var members = obj.GetType().GetMembers(bindingFlags);
            return members.Any(m => Attribute.IsDefined(m, typeof(InjectAttribute)));
        }

        void RegisterProvider(IDependencyProvider provider)
        {
            var methods = provider.GetType().GetMethods(bindingFlags);

            foreach (var method in methods)
            {
                if (!Attribute.IsDefined(method, typeof(ProviderAttribute))) continue;

                var returnType = method.ReturnType;
                var providedInstance = method.Invoke(provider, null);

                if (providedInstance != null)
                    registry.Add(returnType, providedInstance);
                else
                    throw new Exception($"Provider {provider.GetType().Name} returned null for {returnType.Name}");
            }
        }

        static MonoBehaviour[] FindMonoBehaviours()
        {
            return FindObjectsByType<MonoBehaviour>(FindObjectsSortMode.InstanceID);
        }
    }
}