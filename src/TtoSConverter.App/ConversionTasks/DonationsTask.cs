using System;
using System.Linq;
using Ambitus.Rest.Operations;
using Intermediates;
namespace TtoSConverter.App.ConversionTasks
{
	public class DonationsTask : IConversionTask
	{
		private readonly Ambitus.Rest.IApiClient restClient;
		private readonly Ambitus.Soap.IApiClient soapClient;

		public DonationsTask(Ambitus.Rest.IApiClient restClient, Ambitus.Soap.IApiClient soapClient)
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
						var existingOrders = context.Orders
							.Where(ord => ord.CustomerId == customer.CustomerId)
							.Select(ord => ord.OrderId);
						var existingDonations = context.Donations
							.Where(don => existingOrders.Contains(don.OrderId));

						if (existingDonations.Any())
						{
							Console.WriteLine("Skipping Created Donations for {0}", customerId);
							continue;
						}

						var donations = restClient.Send(new GetContributionsQuery(customerId));
						foreach (var donation in donations.Data)
						{
							try
							{
								var donationAmount = donation.ContributionAmount.Value;
								var donationFund = donation.Fund.Id.Value;
								var donationOrder = Guid.NewGuid();
								var donationDateTime = donation.ContributionDateTime.Value;
								var donationChannel = donation.Channel.Description;

								var newOrder = new Order
								{
									CustomerId = customerId.ToString(),
									OrderId = donationOrder.ToString(),
									DateTime = donationDateTime,
									OrderFee = donationAmount,
									SalesChannel = donationChannel
								};

								var newDonation = new Donation
								{
									Amount = donationAmount,
									FundId = donationFund.ToString(),
									OrderId = donationOrder.ToString()
								};

								Console.WriteLine("Adding donation {0} to customer {1}'s order {2}", donation.Id.Value, customerId, donationOrder);
								context.Orders.Add(newOrder);
								context.Donations.Add(newDonation);
								context.SaveChanges();
							}
							catch (Exception e)
							{
								Console.WriteLine("An exception occurred:");
								Console.WriteLine(((System.Data.Entity.Validation.DbEntityValidationException)e).EntityValidationErrors);
								Console.WriteLine(e.ToString());
								Console.WriteLine("Skipping donation.");
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
