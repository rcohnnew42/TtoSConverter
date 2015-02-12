using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TtoSConverter.ConversionTasks;

namespace TtoSConverter
{
	public interface ITasksProvider
	{
		IConversionTask[] GetTasks();
	}
}
