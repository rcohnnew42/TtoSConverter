using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ambitus.Rest.Operations;
using Ambitus.Soap.Methods.Session.CreateSession;
using Ambitus.Soap.Methods.Session.LogIn;
using Ambitus.Soap.Methods.Session.LogInWithNewLogin;

namespace TtoSConverter.App.LoginCustomer
{
	public class LoginOrCreateLoginHandler : ILoginOrCreateLoginHandler
	{
		const int BUSINESS_UNIT_ID = 1;
		const string SELF_USERNAME = "bdrenner@new42.org";
		const int DEFAULT_LOGIN_TYPE_ID = 1;
		const string EMAIL_FORMAT = "temp+{0}@new42.org";
		const int INTERNAL_LOGIN_TYPE_ID = 2;

		private readonly Ambitus.Rest.IApiClient restClient;
		private readonly Ambitus.Soap.IApiClient soapClient;

		public LoginOrCreateLoginHandler(Ambitus.Rest.IApiClient restClient, Ambitus.Soap.IApiClient soapClient)
		{
			this.restClient = restClient;
			this.soapClient = soapClient;
		}
		
		public string Execute(int customerId)
		{
			Console.WriteLine("Getting customer login");
			var getLoginResponse = restClient.Send(new GetWebLoginsQuery(customerId));
			if (!getLoginResponse.IsSuccess)
			{
				Console.WriteLine(getLoginResponse.Failure.ToString());
				return string.Empty;
			}
			var login = getLoginResponse.Data.FirstOrDefault();
			var sessionKey = soapClient.Send(new CreateSessionCommand(BUSINESS_UNIT_ID));
			if (login == null)
			{
				Console.WriteLine("No logins found. Creating login.");

				var loginResult = soapClient.Send(new LogInCommand(sessionKey, SELF_USERNAME, DEFAULT_LOGIN_TYPE_ID));
				if (loginResult.IsSuccess == false)
				{
					Console.WriteLine("Failed to log in as self. Aborting.");
					return string.Empty;
				}

				var now = DateTime.Now;
				var emailAddress = string.Format(EMAIL_FORMAT, now.Ticks);
				var addLoginResult = soapClient.Send(new LogInWithNewLoginCommand(sessionKey, customerId, emailAddress, INTERNAL_LOGIN_TYPE_ID, a =>
				{
					a.Username = emailAddress;
				}));

				if (addLoginResult == false)
				{
					Console.WriteLine("Failed to add login. Skipping constituent.");
					return string.Empty;
				}

				Console.WriteLine("Created login {0}.", emailAddress);
			}
			else
			{
				if (string.IsNullOrWhiteSpace(login.Login))
				{
					Console.WriteLine("Found login is empty or white space. Skipping constituent.");
					return string.Empty;
				}

				Console.WriteLine("Login {0} found. Logging in.", login.Login);

				var loginResult = soapClient.Send(new LogInCommand(sessionKey, login.Login, login.LoginType.Id.Value));

				if (loginResult.IsSuccess == false)
				{
					Console.WriteLine("Failed to log in. Skipping constituent.");
					return string.Empty;
				}
			}

			return sessionKey;
		}
	}
}
