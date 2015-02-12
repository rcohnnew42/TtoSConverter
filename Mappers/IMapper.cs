using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TtoSConverter.Mappers
{
	public interface IMapper<TInput, TOutput>
	{
		TOutput Map(TInput input);
	}
}
