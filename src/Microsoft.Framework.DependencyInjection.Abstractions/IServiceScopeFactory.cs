// Copyright (c) .NET Foundation. All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;

namespace Microsoft.Framework.DependencyInjection
{
    public interface IServiceScopeFactory
    {
        /// <summary>
        /// Create an <see cref="Microsoft.Framework.DependencyInjection.IServiceScope"/> which
        /// contains an <see cref="System.IServiceProvider"/> used to resolve dependencies from a
        /// newly created scope.
        /// </summary>
        /// <param name="buildAdditionalServices">
        /// A <see cref="Action{IServiceCollection}"/> that allows the scope creator to specify
        /// additional services to be resolved by the child scope.
        /// </param>
        /// <returns>
        /// An <see cref="Microsoft.Framework.DependencyInjection.IServiceScope"/> controlling the
        /// lifetime of the scope. Once this is disposed, any scoped services that have been resolved
        /// from the <see cref="Microsoft.Framework.DependencyInjection.IServiceScope.ServiceProvider"/>
        /// will also be disposed.
        /// </returns>
        IServiceScope CreateScope(Action<IServiceCollection> buildAdditionalServices = null);
    }
}
