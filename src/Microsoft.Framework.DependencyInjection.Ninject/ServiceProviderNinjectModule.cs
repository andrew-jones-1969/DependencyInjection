// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using Ninject;
using Ninject.Modules;
using Ninject.Syntax;

namespace Microsoft.Framework.DependencyInjection.Ninject
{
    internal class ServiceProviderNinjectModule : NinjectModule
    {
        private readonly IEnumerable<ServiceDescriptor> _serviceDescriptors;

        public ServiceProviderNinjectModule(
                IEnumerable<ServiceDescriptor> serviceDescriptors)
        {
            _serviceDescriptors = serviceDescriptors;
        }

        public override void Load()
        {
            NinjectServiceBinder.BindServices(this, _serviceDescriptors);

            Bind<IServiceProvider>().ToMethod(context =>
            {
                var resolver = context.Kernel.Get<IResolutionRoot>();
                var inheritedParams = context.Parameters.Where(p => p.ShouldInherit);

                var scopeParam = new ScopeParameter();
                inheritedParams = inheritedParams.AddOrReplaceScopeParameter(scopeParam);

                return new NinjectServiceProvider(resolver, inheritedParams.ToArray());
            }).InRequestScope();

            Bind<IServiceScopeFactory>().ToMethod(context =>
            {
                return new NinjectServiceScopeFactory(context);
            }).InRequestScope();
        }
    }
}
