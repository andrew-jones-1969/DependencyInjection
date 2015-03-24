// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using Microsoft.Practices.Unity;

namespace Microsoft.Framework.DependencyInjection.Unity
{
	internal class ServiceScopeService : IServiceScopeFactory
	{
		private IUnityContainer unity;

		public ServiceScopeService(IUnityContainer unity)
		{
			this.unity = unity;
		}

		IServiceScope IServiceScopeFactory.CreateScope()
		{
			return new ServiceScope(unity);
		}
	}
}