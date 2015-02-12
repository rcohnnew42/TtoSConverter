using System;
using System.Linq;
using Ambitus.Rest.Operations;
using Intermediates;
namespace TtoSConverter.ConversionTasks
{
	public class CustomerTagsTask : IConversionTask
	{
		private readonly Ambitus.Rest.IApiClient restClient;
		private readonly Ambitus.Soap.IApiClient soapClient;

		public CustomerTagsTask(Ambitus.Rest.IApiClient restClient, Ambitus.Soap.IApiClient soapClient)
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
						var existingTags = context.CustomerTags
							.Where(att => att.CustomerId == customer.CustomerId);

						if (existingTags.Any())
						{
							Console.WriteLine("Skipping Created Tags for {0}", customerId);
							continue;
						}

						var customerTagsResult = restClient.Send(new GetConstituenciesQuery(customerId));
						var customerTags = customerTagsResult.Data
							.Where(tag => tag.EndDate == null || tag.EndDate.Value > DateTime.Now);
						foreach (var customerTag in customerTags)
						{
							try
							{
								var customerTagId = customerTag.ConstituencyType.Id;

								var newCustomerTag = new CustomerTag
								{
									CustomerId = customerId.ToString(),
									TagId = customerTagId.ToString()
								};

								Console.WriteLine("Adding tag {0} to customer {1}", customerTagId, customerId);
								context.CustomerTags.Add(newCustomerTag);
								context.SaveChanges();
							}
							catch (Exception e)
							{
								Console.WriteLine("An exception occurred:");
								Console.WriteLine(((System.Data.Entity.Validation.DbEntityValidationException)e).EntityValidationErrors);
								Console.WriteLine(e.ToString());
								Console.WriteLine("Skipping customer tag.");
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
