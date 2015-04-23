using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TtoSConverter.App.ConversionTasks;

namespace TtoSConverter.App
{
	public interface ITasksProvider
	{
		IConversionTask[] GetTasks();
	}
}
