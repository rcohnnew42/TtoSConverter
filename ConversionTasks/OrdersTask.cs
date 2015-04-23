using System;
using System.Linq;
using Ambitus.Rest.Operations;
using Ambitus.Soap.Methods.Account.GetOrder;
using Ambitus.Soap.Methods.Account.GetOrderSummaries;
using Intermediates;
using TtoSConverter.LoginCustomer;
namespace TtoSConverter.ConversionTasks
{
	public class OrdersTask : IConversionTask
	{
		private readonly Ambitus.Rest.IApiClient restClient;
		private readonly Ambitus.Soap.IApiClient soapClient;
		private readonly ILoginOrCreateLoginHandler loginOrCreateLoginHandler;

		public OrdersTask(Ambitus.Rest.IApiClient restClient, Ambitus.Soap.IApiClient soapClient, ILoginOrCreateLoginHandler loginOrCreateLoginHandler)
		{
			this.restClient = restClient;
			this.soapClient = soapClient;
			this.loginOrCreateLoginHandler = loginOrCreateLoginHandler;
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
							.Where(ord => ord.CustomerId == customer.CustomerId);

						if (existingOrders.Any())
						{
							Console.WriteLine("Skipping Created Orders for {0}", customerId);
							continue;
						}

						var sessionKey = this.loginOrCreateLoginHandler.Execute(customerId);
						var orders = soapClient.Send(new GetOrderSummariesQuery(sessionKey,customerId));
						foreach (var order in orders)
						{
							try
							{
								var orderDetails = soapClient.Send(new GetOrderQuery(sessionKey,order.Id.Value));

								var orderId = orderDetails.Id.Value;
								var orderDateTime = orderDetails.OrderDateTime.Value;
								var orderSalesChannel = orderDetails.ChannelName;
								var orderPaymentMethod = orderDetails.Payments == null || !orderDetails.Payments.Any() ? string.Empty : orderDetails.Payments.First().Name;
								var orderPaymentReference = orderDetails.Payments == null || !orderDetails.Payments.Any() ? 0 : orderDetails.Payments.First().Id.Value;
								var orderFee = orderDetails.Payments.Sum(p => p.Amount ?? 0);

								var newOrder = new Order
								{
									OrderId = orderId.ToString(),
									CustomerId = customerId.ToString(),
									DateTime = orderDateTime,
									SalesChannel = orderSalesChannel,
									OrderFee = orderFee,
									PaymentMethod = orderPaymentMethod,
									PaymentReference = orderPaymentReference.ToString()
								};

								Console.WriteLine("Adding order {0} to customer {1}", orderId, customerId);
								context.Orders.Add(newOrder);
								context.SaveChanges();
							}
							catch (Exception e)
							{
								Console.WriteLine("An exception occurred:");
								Console.WriteLine(((System.Data.Entity.Validation.DbEntityValidationException)e).EntityValidationErrors);
								Console.WriteLine(e.ToString());
								Console.WriteLine("Skipping customer order.");
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
