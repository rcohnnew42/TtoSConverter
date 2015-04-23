using System;
using System.Linq;
using Ambitus.Rest.Operations;
using Ambitus.Soap.Methods.Inventory.GetScreens;
using Ambitus.Soap.Methods.Inventory.GetSeats;
using Ambitus.Soap.Methods.Session.CreateSession;
using Intermediates;
namespace TtoSConverter.ConversionTasks
{
	public class SeatingTask : IConversionTask
	{
		const int BUSINESS_UNIT_ID = 1;
				
		private readonly Ambitus.Rest.IApiClient restClient;
		private readonly Ambitus.Soap.IApiClient soapClient;

		public SeatingTask(Ambitus.Rest.IApiClient restClient, Ambitus.Soap.IApiClient soapClient)
		{
			this.restClient = restClient;
			this.soapClient = soapClient;
		}

		public void Execute()
		{
			using (var context = new IntermediatesEntities())
			{

				var sessionKey = soapClient.Send(new CreateSessionCommand(BUSINESS_UNIT_ID));
				foreach (var instance in context.Instances.Where(a=>a.InstanceId!="0"))
				{
					try
					{
						var instanceId = int.Parse(instance.InstanceId);
						var performanceResult = restClient.Send(new GetPerformanceQuery(instanceId));
						var facilityId = performanceResult.Data.Facility.Id.Value;
						var seatsResult = soapClient.Send(new GetSeatsQuery(sessionKey,instanceId,false,false));
					
						foreach (var seat in seatsResult)
						{
							try
							{
								
								
								var seatId = seat.Id.Value;
								var row = seat.Row.Name;
								var number = seat.Number.Name;
								var xCoordinate = seat.Coordinates.XCoordinate.Value;
								var yCoordinate = seat.Coordinates.YCoordinate.Value;
								var seatingAreaId = seat.ScreenId.Value;
								var priceBandId = seat.ZoneId.Value;
								var lockTypeId = seat.HoldCodeId;
								//var screenId = screen.Id.Value;
								//var screenName = screen.Name;
								//var seatMapIdAsString = seatMapId.ToString();
								var seatIdAsString = seatId.ToString();
								var existingSeats = context.Seats
									.Where(seats => seats.SeatId == seatIdAsString);

								if (existingSeats.Any())
								{
									Console.WriteLine("Skipping Created SeatId {0}", seatId);
									continue;
								}

								var newSeat = new Seat
								{
									SeatId = seatId.ToString(),
									Row = row,
									Number = int.Parse(number),
									CoordinatesX = xCoordinate,
									CoordinatesY = yCoordinate,
									SeatingAreaId = seatingAreaId.ToString(),
									ParentAreaId = facilityId.ToString(),
									PriceBandId = priceBandId.ToString()
								};

								Console.WriteLine("Adding Seat {0} into Seating Area {1}/{2} for Instance {3}", seatId, seatingAreaId,facilityId,instanceId);
								context.Seats.Add(newSeat);
								
								if (lockTypeId.HasValue)
								{
									var newLockedSeat = new LockedSeat
									{
										SeatId = seatId.ToString(),
										LockTypeId = lockTypeId.Value.ToString()
									};
									context.LockedSeats.Add(newLockedSeat);
								}
								//context.SaveChanges();

							}
							catch (Exception e)
							{
								Console.WriteLine("An exception occurred:");
								Console.WriteLine(((System.Data.Entity.Validation.DbEntityValidationException)e).EntityValidationErrors);
								Console.WriteLine(e.ToString());
								Console.WriteLine("Skipping seat.");
							}
						}

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
		}
	}
}
