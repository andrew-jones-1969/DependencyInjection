// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Framework.DependencyInjection.ServiceLookup
{
    internal class ServiceScopeFactory : IServiceScopeFactory
    {
        private readonly ServiceProvider _provider;

        public ServiceScopeFactory(ServiceProvider provider)
        {
            _provider = provider;
        }

        public IServiceScope CreateScope(Action<IServiceCollection> buildAdditionalServices = null)
        {
            ServiceProvider resultProvider;
            if (buildAdditionalServices != null)
            {
                var additionalServices = new ServiceCollection();
                buildAdditionalServices(additionalServices);
                resultProvider = new ServiceProvider(_provider, additionalServices);
            }
            else
            {
                resultProvider = new ServiceProvider(_provider);
            }
            return new ServiceScope(resultProvider);
        }
    }
}
