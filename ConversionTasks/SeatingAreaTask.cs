using System;
using System.Linq;
using Ambitus.Rest.Operations;
using Ambitus.Soap.Methods.Inventory.GetScreens;
using Ambitus.Soap.Methods.Session.CreateSession;
using Intermediates;
namespace TtoSConverter.ConversionTasks
{
	public class SeatingAreaTask : IConversionTask
	{
		const int BUSINESS_UNIT_ID = 1;
				
		private readonly Ambitus.Rest.IApiClient restClient;
		private readonly Ambitus.Soap.IApiClient soapClient;

		public SeatingAreaTask(Ambitus.Rest.IApiClient restClient, Ambitus.Soap.IApiClient soapClient)
		{
			this.restClient = restClient;
			this.soapClient = soapClient;
		}

		public void Execute()
		{
			using (var context = new IntermediatesEntities())
			{

				var facilitiesResult = restClient.Send(new GetFacilitiesQuery());
				foreach (var facility in facilitiesResult.Data)
				{
					try
					{
						var seatingPlanId = facility.SeatMap.Id.Value;
						var seatingPlanIdAsString = seatingPlanId.ToString();
						var existingSeatingPlans = context.SeatingAreas
							.Where(seats => seats.SeatingAreaId == seatingPlanIdAsString);

						if (existingSeatingPlans.Any())
						{
							Console.WriteLine("Skipping Created SeatingAreas for {0}", seatingPlanId);
							continue;
						}
						
						var seatingPlanDescription = facility.SeatMap.Description;

						var newSeatingArea = new SeatingArea
						{
							SeatingAreaId = seatingPlanId.ToString(),
							Name = seatingPlanDescription,
							ParentAreaId = string.Empty,
							TopLevelAreaId = seatingPlanId.ToString(),
							Type = "Group"
						};

						Console.WriteLine("Adding SeatingArea {0}", seatingPlanId);
						context.SeatingAreas.Add(newSeatingArea);
						context.SaveChanges();
					}
					catch (Exception e)
					{
						Console.WriteLine("An exception occurred:");
						Console.WriteLine(((System.Data.Entity.Validation.DbEntityValidationException)e).EntityValidationErrors);
						Console.WriteLine(e.ToString());
						Console.WriteLine("Skipping facility.");
					}
				}

				var sessionKey = soapClient.Send(new CreateSessionCommand(BUSINESS_UNIT_ID));
				foreach (var instance in context.Instances)
				{
					try
					{
						var instanceId = int.Parse(instance.InstanceId);
						var screensResult = soapClient.Send(new GetScreensQuery(sessionKey,instanceId));
					
						foreach (var screen in screensResult)
						{
							try
							{
								var seatMapId = screen.SeatMap.Id.Value;
								var screenId = screen.Id.Value;
								var screenName = screen.Name;
								var seatMapIdAsString = seatMapId.ToString();
								var screenIdAsString = screenId.ToString();
								var existingSeatingAreas = context.SeatingAreas
									.Where(seats => seats.SeatingAreaId == screenIdAsString && seats.ParentAreaId == seatMapIdAsString);

								if (existingSeatingAreas.Any())
								{
									Console.WriteLine("Skipping Created SeatingAreas for {0}/{1}", screenId,seatMapId);
									continue;
								}

								var newSeatingArea = new SeatingArea
								{
									SeatingAreaId = screenId.ToString(),
									Name = screenName,
									ParentAreaId = seatMapId.ToString(),
									TopLevelAreaId = seatMapId.ToString(),
									Type = "Reserved"
								};

								Console.WriteLine("Adding SeatingArea {0}/{1}", screenId, seatMapId);
								context.SeatingAreas.Add(newSeatingArea);
								//context.SaveChanges();

							}
							catch (Exception e)
							{
								Console.WriteLine("An exception occurred:");
								Console.WriteLine(((System.Data.Entity.Validation.DbEntityValidationException)e).EntityValidationErrors);
								Console.WriteLine(e.ToString());
								Console.WriteLine("Skipping screen.");
							}
						}

						context.SaveChanges();
					}
					catch (Exception e)
					{
						Console.WriteLine("An exception occurred:");
						Console.WriteLine(((System.Data.Entity.Validation.DbEntityValidationException)e).EntityValidationErrors);
						Console.WriteLine(e.ToString());
						Console.WriteLine("Skipping seatmap.");
					}
					
				}
			}
		}
	}
}
