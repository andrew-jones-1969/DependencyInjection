using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ninject;
using Ninject.Syntax;

namespace Microsoft.Framework.DependencyInjection.Ninject
{
    internal static class NinjectServiceBinder
    {
        internal static void BindServices(IBindingRoot target, IEnumerable<ServiceDescriptor> _serviceDescriptors)
        {
            foreach (var descriptor in _serviceDescriptors)
            {
                IBindingWhenInNamedWithOrOnSyntax<object> binding;

                if (descriptor.ImplementationType != null)
                {
                    binding = target.Bind(descriptor.ServiceType).To(descriptor.ImplementationType);
                }
                else if (descriptor.ImplementationFactory != null)
                {
                    binding = target.Bind(descriptor.ServiceType).ToMethod(context =>
                    {
                        var serviceProvider = context.Kernel.Get<IServiceProvider>();
                        return descriptor.ImplementationFactory(serviceProvider);
                    });
                }
                else
                {
                    binding = target.Bind(descriptor.ServiceType).ToConstant(descriptor.ImplementationInstance);
                }

                switch (descriptor.Lifetime)
                {
                    case ServiceLifetime.Singleton:
                        binding.InSingletonScope();
                        break;
                    case ServiceLifetime.Scoped:
                        binding.InRequestScope();
                        break;
                    case ServiceLifetime.Transient:
                        binding.InTransientScope();
                        break;
                }
            }
        }
    }
}
