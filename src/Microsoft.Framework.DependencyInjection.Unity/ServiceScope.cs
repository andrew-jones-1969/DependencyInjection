// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Practices.Unity;

namespace Microsoft.Framework.DependencyInjection.Unity
{
	internal class ServiceScope : IServiceScope
	{
		private readonly IUnityContainer parentContainer;
		private IUnityContainer unityContainer;
		private bool disposedValue = false;	// To detect redundant calls
		private IServiceProvider serviceProvider;

		public ServiceScope(IUnityContainer parentContainer)
		{
			this.parentContainer = parentContainer;
		}

		IServiceProvider IServiceScope.ServiceProvider
		{
			get
			{
				if (serviceProvider == null)
				{
					if (unityContainer == null)
						unityContainer = parentContainer.CreateChildContainer();
					serviceProvider = new UnityServiceProvider(unityContainer);
				}
				return serviceProvider;
			}
		}

		#region IDisposable Support

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					if (unityContainer != null)
					{
						unityContainer.Dispose();
					}
				}


				disposedValue = true;
			}
		}

		void IDisposable.Dispose()
		{
			Dispose(true);
		}
		#endregion

	}
}