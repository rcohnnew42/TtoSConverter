using System;
using System.Collections.Generic;
using System.Linq;
using Ambitus.Rest.Operations;
using Intermediates;
using Ninject;

namespace TtoSConverter.App
{
	class Program
	{
		private static void Main()
		{
			var dependencyBindings = new DependencyBindings();
			var container = new StandardKernel(dependencyBindings);
			var taskProvider = container.Get<ITasksProvider>();

			var tasks = taskProvider.GetTasks();
			foreach (var task in tasks)
			{
				Console.WriteLine("Executing task {0}", task.GetType().Name);
				try
				{
					task.Execute();
				}
				catch (Exception e)
				{
					Console.WriteLine("An exception occurred:");
					Console.WriteLine(e.Message);
					char key = default(char);
					while (key != 'y' && key != 'n')
					{
						Console.WriteLine("Continue running tasks? y/n");
						key = Console.ReadKey().KeyChar;
					}
					if (key == 'n')
					{
						break;
					}
				}
			}
			Console.ReadKey();
		}
	}
}
		
	//	static void AltMain(string[] args)
	//	{
	//		var soapClient = new Ambitus.Soap.ApiClient();
	//		var restClient = new Ambitus.Rest.ApiClient(); 
	//		//var context = new IntermediatesEntities();

	//		//var list = new List()
	//		//var createListofAllCustomers = restClient.Send(new CreateListCommand(Ambitus.Rest.Models.List);
			

			

				
				
	//		}
	//	}
	//}


