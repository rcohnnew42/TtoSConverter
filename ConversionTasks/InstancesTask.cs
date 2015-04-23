using System;
using System.Collections.Generic;
using System.Data.Entity.SqlServer;
using System.Linq;
using Ambitus.Rest.Operations;
using Intermediates;
namespace TtoSConverter.ConversionTasks
{
	public class InstancesTask : IConversionTask
	{
		private readonly Ambitus.Rest.IApiClient restClient;
		private readonly Ambitus.Soap.IApiClient soapClient;

		public InstancesTask(Ambitus.Rest.IApiClient restClient, Ambitus.Soap.IApiClient soapClient)
		{
			this.restClient = restClient;
			this.soapClient = soapClient;
		}

		public void Execute()
		{
			using (var context = new IntermediatesEntities())
			{

				var seasonsResult = restClient.Send(new GetSeasonsQuery());
				var seasons = seasonsResult.Data
					.Select(sea => sea.Id.Value);

				foreach (var season in seasons)
				{

					try
					{

						var instancesResults = restClient.Send(new GetPerformancesQuery(season));
						var instances = instancesResults.Data;
						var existingInstances = context.Instances.Select(inst => inst.InstanceId);

						foreach (var instance in instances)
						{
							try
							{
								if (existingInstances.Contains(instance.Id.Value.ToString()))
								{
									Console.WriteLine("Skipping Existing Instance {0}", instance.Id.Value);
									continue;
								}
								
								var priceListId = Guid.NewGuid();
								var priceListName = Guid.NewGuid();

								var newPriceList = new PriceList
								{
									PriceListId = priceListId.ToString(),
									Name = priceListName.ToString()
								};

								Console.WriteLine("Adding pricelist {0}", priceListId);
								context.PriceLists.Add(newPriceList);
								context.SaveChanges();

								var instanceTicketTypesResult = restClient.Send(new GetPerformancePriceTypesQuery(instance.Id.Value));
								var instanceTicketTypes = instanceTicketTypesResult.Data;

								foreach (var instanceTicketType in instanceTicketTypes)
								{
									try
									{
										var instancePrices = instanceTicketType.PerformancePrices;

										foreach (var instancePrice in instancePrices)
										{
											try
											{

												var priceListEntryId = instancePrice.Id.Value;
												var priceListEntryAmount = instancePrice.Price.Value;
												var priceListPriceBandId = instancePrice.Zone.Value.ToString();
												var priceListTicketTypeId = instanceTicketType.PriceType.ToString();

												//if (context.PriceListEntries
												//	.Where(pe => pe.PriceBandId == priceListPriceBandId && pe.TicketTypeId == priceListTicketTypeId)
												//	.Any())
												//{
												//	Console.WriteLine("Skipping PriceListEntry for priceband {0} and tickettype {1}", priceListPriceBandId, priceListTicketTypeId);
												//	continue;
												//}
												
												var newPriceListEntry = new PriceListEntry
												{
													PriceListId = priceListId.ToString(),
													Price = priceListEntryAmount,
													PriceBandId = priceListPriceBandId.ToString(),
													TicketTypeId = priceListTicketTypeId.ToString()
												};

												Console.WriteLine("Adding pricelistentry {0}", instancePrice.Id.Value);
												context.PriceListEntries.Add(newPriceListEntry);
												context.SaveChanges();
											}
											catch (Exception e)
											{
												Console.WriteLine("An exception occurred:");
												Console.WriteLine(((System.Data.Entity.Validation.DbEntityValidationException)e).EntityValidationErrors);
												Console.WriteLine(e.ToString());
												Console.WriteLine("Skipping pricelistentry.");
											}
										}
									}
									catch (Exception e)
									{
										Console.WriteLine("An exception occurred:");
										Console.WriteLine(((System.Data.Entity.Validation.DbEntityValidationException)e).EntityValidationErrors);
										Console.WriteLine(e.ToString());
										Console.WriteLine("Skipping pricelist.");
									}
								}

								var instanceId = instance.Id.Value;
								var eventId = instance.ProductionSeason.Id.Value;
								var startDateTime = instance.Date.Value;
								var notes = new System.Text.StringBuilder();
								notes.Append(instance.Text1);
								notes.Append(instance.Text2);
								notes.Append(instance.Text3);
								notes.Append(instance.Text4);
								var seatingMapId = instance.Facility.SeatMap.Id.Value;

								var newInstance = new Instance
								{
									InstanceId = instanceId.ToString(),
									EventId = eventId.ToString(),
									Start = startDateTime,
									Notes = notes.ToString(),
									SeatingPlanId = seatingMapId.ToString(),
									PriceListId = priceListId.ToString(),
									TicketDesignId = "",
									CommissionStructureId = ""
								};

								Console.WriteLine("Adding instance {0}", instanceId);
								context.Instances.Add(newInstance);
								context.SaveChanges();
							}
							catch (Exception e)
							{
								Console.WriteLine("An exception occurred:");
								Console.WriteLine(((System.Data.Entity.Validation.DbEntityValidationException)e).EntityValidationErrors);
								Console.WriteLine(e.ToString());
								Console.WriteLine("Skipping instance.");
							}
						}
					}
					catch (Exception e)
					{
						Console.WriteLine("An exception occurred:");
						Console.WriteLine(((System.Data.Entity.Validation.DbEntityValidationException)e).EntityValidationErrors);
						Console.WriteLine(e.ToString());
						Console.WriteLine("Skipping season.");
					}
				}
			}
		}
	}
}

