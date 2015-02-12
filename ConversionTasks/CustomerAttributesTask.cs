using System;
using System.Linq;
using Ambitus.Rest.Operations;
using Intermediates;
namespace TtoSConverter.ConversionTasks
{
	public class CustomerAttributesTask : IConversionTask
	{
		private readonly Ambitus.Rest.IApiClient restClient;
		private readonly Ambitus.Soap.IApiClient soapClient;

		public CustomerAttributesTask(Ambitus.Rest.IApiClient restClient, Ambitus.Soap.IApiClient soapClient)
		{
			this.restClient = restClient;
			this.soapClient = soapClient;
		}

		public void Execute()
		{
			using (var context = new IntermediatesEntities())
			{

				foreach (var customer in context.Customers)
				{
					try
					{
						var customerId = int.Parse(customer.CustomerId);
						var existingAttributes = context.CustomerAttributes
							.Where(att => att.CustomerId == customer.CustomerId);

						if (existingAttributes.Any())
						{
							Console.WriteLine("Skipping Created Attributes for {0}", customerId);
							continue;
						}

						var customerAttributes = restClient.Send(new GetAttributesQuery(customerId));
						foreach (var customerAttribute in customerAttributes.Data)
						{
							try
							{
								var customerAttributeDescription = customerAttribute.Keyword.Description;
								var customerAttributeValue = customerAttribute.Value;

								var newCustomerAttribute = new CustomerAttribute
								{
									CustomerId = customerId.ToString(),
									Name = customerAttributeDescription,
									Value = customerAttributeValue
								};

								Console.WriteLine("Adding attribute {0} to customer {1}", customerAttributeDescription, customerId);
								context.CustomerAttributes.Add(newCustomerAttribute);
								context.SaveChanges();
							}
							catch (Exception e)
							{
								Console.WriteLine("An exception occurred:");
								Console.WriteLine(((System.Data.Entity.Validation.DbEntityValidationException)e).EntityValidationErrors);
								Console.WriteLine(e.ToString());
								Console.WriteLine("Skipping customer attribute.");
							}
						}
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
