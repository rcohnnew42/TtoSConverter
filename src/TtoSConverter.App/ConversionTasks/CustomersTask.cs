using System;
using System.Linq;
using Ambitus.Rest.Operations;
using Intermediates;
namespace TtoSConverter.App.ConversionTasks
{
	public class CustomersTask : IConversionTask
	{
		private readonly Ambitus.Rest.IApiClient restClient;
		private readonly Ambitus.Soap.IApiClient soapClient;

		public CustomersTask(Ambitus.Rest.IApiClient restClient, Ambitus.Soap.IApiClient soapClient)
		{
			this.restClient = restClient;
			this.soapClient = soapClient;
		}

		public void Execute()
		{
			const int LIST_OF_ALL_CUSTOMERS = 29043;

			using (var context = new IntermediatesEntities())
			{

				var getListofAllCustomersFromT = restClient.Send(new GetListContentsQuery(LIST_OF_ALL_CUSTOMERS));
				var getListofAllCustomersFromS = context.Customers
					.Select(cust => cust.CustomerId);
				var allCustomerIdsFromT = getListofAllCustomersFromT.Data
					.Where(cust => !getListofAllCustomersFromS.Contains(cust.ConstituentId.Value.ToString()) && cust.ConstituentId.Value < 1000)
					.Select(cust => cust.ConstituentId.Value);

				foreach (var customerId in allCustomerIdsFromT)
				{
					try
					{
						var customerDetails = restClient.Send(new GetConstituentDetailsQuery(customerId));
						var customerType = customerDetails.Data.ConstituentType.Description;
						var customer = new Customer
						{
							CustomerId = customerId.ToString(),
							Type = customerType
						};
						Console.WriteLine("Adding Customer {0}", customerId);
						context.Customers.Add(customer);
						context.SaveChanges();
					}
					catch (Exception e)
					{
						Console.WriteLine("An exception occurred:");
						Console.WriteLine(((System.Data.Entity.Validation.DbEntityValidationException)e).EntityValidationErrors);
						Console.WriteLine(e.ToString());
						Console.WriteLine("Skipping customer.");
					}
				}

				foreach (var customer in context.Customers)
				{
					try
					{
						if (customer.Created != null)
						{
							Console.WriteLine("Skipping Created Customer", customer.CustomerId);
							continue;
						}

						// Add Customer Name

						var customerId = int.Parse(customer.CustomerId);
						var customerDetails = restClient.Send(new GetConstituentDetailsQuery(customerId));
						var customerFirstName = customerDetails.Data.FirstName;
						var customerLastName = customerDetails.Data.LastName;

						// Add Customer Primary Address

						var customerPrimaryAddress = customerDetails.Data.Addresses
							.Where(add => add.PrimaryIndicator.Value == true)
							.First();
						var customerAddressLine1 = customerPrimaryAddress.Street1;
						var customerAddressLine2 = customerPrimaryAddress.Street2;
						var customerAddressLine3 = customerPrimaryAddress.Street3;
						var customerPostTown = customerPrimaryAddress.City;
						var customerCounty = customerPrimaryAddress.State == null ? string.Empty : customerPrimaryAddress.State.Description;
						var customerCountry = customerPrimaryAddress.Country == null ? string.Empty : customerPrimaryAddress.Country.Description;
						var customerPostCode = customerPrimaryAddress.PostalCode;

						// Add Customer Create/Update Details

						var customerCreateDateTime = customerDetails.Data.CreatedDateTime.Value;
						var customerLastUpdatedDateTime = customerDetails.Data.UpdatedDateTime.Value;

						// Add Customer Email

						var customerEmailAddress =
							customerDetails.Data.ElectronicAddresses.Where(email => email.PrimaryIndicator.Value == true).Any() == false ?
							null :
							customerDetails.Data.ElectronicAddresses.Where(email => email.PrimaryIndicator.Value == true).First().Address;

						// Add Phone

						//var customerPhone =
						//	customerDetails.Data.PhoneNumbers.Where(phone => phone.Primary.Value == true).Any() == false ?
						//	null :
						//	customerDetails.Data.ElectronicAddresses.Where(email => email.PrimaryIndicator.Value == true).First().Address;

						Console.WriteLine("Update Customer {0}: {1} {2}", customer.CustomerId, customerFirstName, customerLastName);
						customer.FirstName = customerFirstName;
						customer.LastName = customerLastName;
						customer.AddressLine1 = customerAddressLine1;
						customer.AddressLine2 = customerAddressLine2;
						customer.AddressLine3 = customerAddressLine3;
						customer.PostTown = customerPostTown;
						customer.County = customerCounty;
						customer.Country = customerCountry;
						customer.Postcode = customerPostCode;
						customer.Created = customerCreateDateTime;
						customer.LastUpdated = customerLastUpdatedDateTime;
						customer.EmailAddress = customerEmailAddress;
						context.SaveChanges();
					}
					catch (Exception e)
					{
						Console.WriteLine("An exception occurred:");
						Console.WriteLine(((System.Data.Entity.Validation.DbEntityValidationException)e).EntityValidationErrors);
						Console.WriteLine(e.ToString());
						Console.WriteLine("Skipping customer.");
					}
				}
			}
		}
	}
}
