using System;

namespace TtoSConverter.LoginCustomer
{
	public interface ILoginOrCreateLoginHandler
	{
		string Execute(int customerId);
	}
}
