using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ninject;
using TtoSConverter.ConversionTasks;

namespace TtoSConverter
{
	public class TaskProvider : ITasksProvider
	{
		private readonly IKernel container;

		public TaskProvider(IKernel container)
		{
			this.container = container;
		}

		private IConversionTask Get<T>()
			where T : IConversionTask
		{
			return this.container.Get<T>();
		}

		private IConversionTask[] tasks;

		public IConversionTask[] GetTasks()
		{
			if (tasks == null)
			{
				tasks = new IConversionTask[]
				{
					//Get<DummyTask>(),
					//Get<Dummy2Task>(),
					//Get<CustomersTask>(),
					//Get<CustomerAttributesTask>(),
					//Get<CustomerTagsTask>(),
					//Get<FundsTask>(),
					//Get<EventsTask>(),
					//Get<OrdersTask>(),
					//Get<DonationsTask>(),
					//Get<SeatingAreaTask>(),
					Get<SeatingTask>()
					//Get<PriceBandsTask>(),
					//Get<TicketsTask>(),
					//Get<LockTypesTask>(),
					//Get<InstancesTask>()
				};
			}
			return tasks;
		}
	}
}
