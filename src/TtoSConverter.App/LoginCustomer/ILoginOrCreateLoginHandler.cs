using System;

namespace TtoSConverter.App.LoginCustomer
{
	public interface ILoginOrCreateLoginHandler
	{
		string Execute(int customerId);
	}
}
