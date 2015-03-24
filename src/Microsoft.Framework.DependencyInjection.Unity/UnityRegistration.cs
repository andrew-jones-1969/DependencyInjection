// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.Practices.Unity;

namespace Microsoft.Framework.DependencyInjection.Unity
{
    public static class UnityRegistration
    {

		public static IServiceProvider Populate(this IUnityContainer unity, IEnumerable<IServiceDescriptor> services)
		{
			var registrations = new List<ContainerRegistration>();
			foreach (var serviceGroup in services.GroupBy(descriptor => descriptor.ServiceType))
			{
				var group = serviceGroup.ToArray();
				var isGroup = group.Length > 1;
				for (var index = 0; index < group.Length; index++)
				{
					var service = group[index];
					LifetimeManager lifetimeManager = GetLifetimeManager(service.Lifecycle);
					string name = GetName(index);

					if (index == group.Length - 1)
					{
						// Last service needs to win; if there's multiple, use a name up until the last one.
						// Still register the name for IEnumerable<> resolvings, as only named registrations resolve for ResolveAll.
						unity.RegisterType(service.ServiceType, name, new InjectionFactory(uc => uc.Resolve(service.ServiceType)));
						Register(service, lifetimeManager, null, unity);
					}
					else
					{
						Register(service, lifetimeManager, name, unity);
					}
				}
				if (registrations.Count < unity.Registrations.Count())
				{
					registrations.AddRange(unity.Registrations.Skip(registrations.Count));
				}
			}

			unity.RegisterInstance<IServiceScopeFactory>(new ServiceScopeService(unity));
			unity.RegisterType(typeof(IEnumerable<>), new InjectionFactory(ResolveEnumerable));

			return new UnityServiceProvider(unity);
		}

		private static void Register(IServiceDescriptor service, LifetimeManager lifetimeManager, string name, IUnityContainer unity)
		{
			if (service.ImplementationFactory != null)
			{
				var factory = service.ImplementationFactory;

				unity.RegisterType(service.ServiceType, name, lifetimeManager, new InjectionFactory(uc => factory(uc.Resolve<IServiceProvider>())));
			}
			else if (service.ImplementationInstance != null)
			{
				unity.RegisterInstance(service.ServiceType, name, service.ImplementationInstance, lifetimeManager);
			}
			else if (service.ImplementationType != null && service.ImplementationType != service.ServiceType)
			{
				// Since unity by default uses ImplementationType for the BuildKey, we move it over to the InjectionFactory.
				unity.RegisterType(service.ServiceType, name, lifetimeManager, new InjectionFactory(ResolveByImplementationType(service.ImplementationType)));
			}
			else
			{
				unity.RegisterType(service.ServiceType, name, lifetimeManager);
			}
		}

		private static Func<IUnityContainer, Type, string, object> ResolveByImplementationType(Type implementationType)
		{
			if (implementationType.IsGenericTypeDefinition)
			{
				return (container, inType, name) => container.Resolve(implementationType.MakeGenericType(inType.GetGenericArguments()));
			}
			else
			{
				return (container, inType, name) => container.Resolve(implementationType);
			}
		}

		private static string GetName(int index)
		{
			return "index " + index.ToString();
		}

		private static LifetimeManager GetLifetimeManager(LifecycleKind lifecycle)
		{
			switch (lifecycle)
			{
				case LifecycleKind.Scoped:
					return new HierarchicalLifetimeManager();
				case LifecycleKind.Singleton:
					return new ContainerControlledLifetimeManager();
				case LifecycleKind.Transient:
					return new TransientLifetimeManager();
				default:
					throw new NotImplementedException();
			}
		}

		private static object ResolveEnumerable(IUnityContainer arg, Type type, string name)
		{
			// type is IEnumerable<>
			return arg.ResolveAll(type.GetGenericArguments()[0]);
		}
	}
}