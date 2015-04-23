using System;
using System.Linq;
using Ambitus.Rest.Operations;
using Ambitus.Soap.Methods.Account.GetOrder;
using Ambitus.Soap.Methods.Account.GetOrderedItemSummaries;
using Ambitus.Soap.Methods.Account.GetOrderSummaries;
using Ambitus.Soap.Methods.Account.GetReturnableTickets;
using Intermediates;
using TtoSConverter.LoginCustomer;
namespace TtoSConverter.ConversionTasks
{
	public class TicketsTask : IConversionTask
	{
		private readonly Ambitus.Rest.IApiClient restClient;
		private readonly Ambitus.Soap.IApiClient soapClient;
		private readonly ILoginOrCreateLoginHandler loginOrCreateLoginHandler;

		public TicketsTask(Ambitus.Rest.IApiClient restClient, Ambitus.Soap.IApiClient soapClient, ILoginOrCreateLoginHandler loginOrCreateLoginHandler)
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
						var sessionKey = this.loginOrCreateLoginHandler.Execute(customerId);
						if (sessionKey == string.Empty)
						{
							continue;
						}
						var tickets = soapClient.Send(new GetReturnableTicketsQuery(sessionKey));
						foreach (var ticket in tickets)
						{
							try
							{
								var ticketId = ticket.Id.Value;
								var instanceId = ticket.Performance.Id.Value;
								var instanceIdAsString = instanceId.ToString();
								var seatId = ticket.Seat.Id.Value;
								var seatIdAsString = seatId.ToString();
								var price = ticket.DueAmount.Value;
								var orderId = ticket.Order.Id.Value;
								var ticketTypeId = ticket.PriceType.Id.Value;
								var existingTickets = context.Tickets
									.Where(tix => tix.InstanceId == instanceIdAsString && tix.SeatId == seatIdAsString);

								if (existingTickets.Any())
								{
									Console.WriteLine("Skipping Created Ticket {0}", ticketId);
									continue;
								}

								var newTicket = new Ticket
								{
									Id = ticketId,
									InstanceId = instanceIdAsString,
									SeatId = seatIdAsString,
									Price = price,
									Printed = true,
									TicketTypeId = ticketTypeId.ToString(),
									OrderId = orderId.ToString()
								};

								Console.WriteLine("Adding ticket {0} to customer {1}", ticketId, customerId);
								context.Tickets.Add(newTicket);
							}
							catch (Exception e)
							{
								Console.WriteLine("An exception occurred:");
								Console.WriteLine(((System.Data.Entity.Validation.DbEntityValidationException)e).EntityValidationErrors);
								Console.WriteLine(e.ToString());
								Console.WriteLine("Skipping customer order.");
							}
						}

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
