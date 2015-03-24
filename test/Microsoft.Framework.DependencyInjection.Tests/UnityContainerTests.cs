// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.

using System;
using Microsoft.Framework.DependencyInjection.Unity;
using Microsoft.Framework.DependencyInjection.Tests.Fakes;
using Microsoft.Practices.Unity;

namespace Microsoft.Framework.DependencyInjection.Tests
{
	public class UnityContainerTests : ScopingContainerTestBase
	{
		protected override IServiceProvider CreateContainer()
		{
			var builder = new UnityContainer();

			return builder.Populate(TestServices.DefaultServices());
		}
	}
}