// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Practices.Unity;

namespace Microsoft.Framework.DependencyInjection.Unity
{
	internal class UnityServiceProvider : IServiceProvider
	{
		private readonly IUnityContainer unityContainer;

		public UnityServiceProvider(IUnityContainer unityContainer)
		{
			this.unityContainer = unityContainer;
			unityContainer.RegisterInstance<IServiceProvider>(this);
		}

		object IServiceProvider.GetService(Type serviceType)
		{
			try
			{
				if (serviceType.IsGenericType)
				{
					// https://unity.codeplex.com/discussions/260188
					// A hierarchical lifetime manager doesn't appear to properly resolve generic mappings until it is resolved at least once.
					// I tried to handle this inside the factory, but it only works outside of the resolve itself.
					// https://unity.codeplex.com/workitem/11014
					unityContainer.Resolve(serviceType);
				}

				return unityContainer.Resolve(serviceType);
			}
			catch (ResolutionFailedException ex)
			{
				if (CatchInternalException(ex, serviceType))
				{
					var exception = ex;
					return null;
				}
				throw;
			}
		}

		private bool CatchInternalException(ResolutionFailedException ex, Type serviceType)
		{
			Exception unwrapped = ex;
			// InnerException will be InvalidOperationException if the serviceType could not be resolved. Otherwise, it'll be a ResolutionFailedException for a dependency.
			if (ex.InnerException is InvalidOperationException)
				return true;
			return false;
		}
	}
}