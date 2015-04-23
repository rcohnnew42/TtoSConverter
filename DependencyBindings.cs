using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject.Modules;
using TtoSConverter.LoginCustomer;

namespace TtoSConverter
{
	public class DependencyBindings : NinjectModule
	{
		public override void Load()
		{
			var restClient = new Ambitus.Rest.ApiClient();
			var soapClient = new Ambitus.Soap.ApiClient();

			Bind<Ambitus.Rest.IApiClient>()
				.ToConstant(restClient);
			Bind<Ambitus.Soap.IApiClient>()
				.ToConstant(soapClient);
			Bind<ILoginOrCreateLoginHandler>()
				.To<LoginOrCreateLoginHandler>()
				.InSingletonScope();
			Bind<ITasksProvider>()
				.To<TaskProvider>()
				.InSingletonScope();
		}
	}
}
