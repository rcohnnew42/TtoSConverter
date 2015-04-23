using System;
using System.Collections.Generic;
using System.Linq;
using Ambitus.Rest.Operations;
using Intermediates;
namespace TtoSConverter.ConversionTasks
{
	public class EventsTask : IConversionTask
	{
		private readonly Ambitus.Rest.IApiClient restClient;
		private readonly Ambitus.Soap.IApiClient soapClient;

		public EventsTask(Ambitus.Rest.IApiClient restClient, Ambitus.Soap.IApiClient soapClient)
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

						var seasonIds = new List<int>();
						seasonIds.Add(season);
						var eventsResult = restClient.Send(new GetProductionSeasonsQuery(seasonIds));
						var existingEvents = context.Events.Select(eve => eve.EventId);

						var events = eventsResult.Data;

						foreach (var eventvar in events)
						{
							try
							{
								var eventId = eventvar.Id.Value;
								var eventName = eventvar.Description;
								var eventCreateDateTime = eventvar.CreatedDateTime.Value;
								//var eventDuration = eventvar.
								var eventDescription = eventvar.Fulltext;

								if (existingEvents.Contains(eventId.ToString()))
								{
									Console.WriteLine("Skipping Created Event {0}", eventId);
									continue;
								}



								var newEvent = new Event
								{
									EventId = eventId.ToString(),
									Name = eventName,
									DateCreated = eventCreateDateTime,
									Description = eventDescription
								};

								Console.WriteLine("Adding event {0}", eventId);
								context.Events.Add(newEvent);
								context.SaveChanges();
							}
							catch (Exception e)
							{
								Console.WriteLine("An exception occurred:");
								Console.WriteLine(((System.Data.Entity.Validation.DbEntityValidationException)e).EntityValidationErrors);
								Console.WriteLine(e.ToString());
								Console.WriteLine("Skipping event.");
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

