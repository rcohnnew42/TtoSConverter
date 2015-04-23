using System;
using System.Linq;
using Ambitus.Rest.Operations;
using Intermediates;
namespace TtoSConverter.ConversionTasks
{
	public class PriceBandsTask : IConversionTask
	{
		private readonly Ambitus.Rest.IApiClient restClient;
		private readonly Ambitus.Soap.IApiClient soapClient;

		public PriceBandsTask(Ambitus.Rest.IApiClient restClient, Ambitus.Soap.IApiClient soapClient)
		{
			this.restClient = restClient;
			this.soapClient = soapClient;
		}

		public void Execute()
		{
			using (var context = new IntermediatesEntities())
			{

				
						var priceBandsResult = restClient.Send(new GetZonesQuery());
						var existingPriceBands = context.PriceBands.Select(pb => pb.PriceBandId);

						var priceBands = priceBandsResult.Data;

						foreach (var priceBand in priceBands)
						{
							try
							{
								var priceBandId = priceBand.Id.Value;
								var priceBandDescription = priceBand.Description;

								if (existingPriceBands.Contains(priceBandId.ToString()))
								{
									Console.WriteLine("Skipping Created PriceBand {0}", priceBandId);
									continue;
								}



								var newPriceBand = new PriceBand
								{
									PriceBandId = priceBandId.ToString(),
									Name = priceBandDescription
								};

								Console.WriteLine("Adding priceBand {0}", priceBandId);
								context.PriceBands.Add(newPriceBand);
								context.SaveChanges();
							}
							catch (Exception e)
							{
								Console.WriteLine("An exception occurred:");
								Console.WriteLine(((System.Data.Entity.Validation.DbEntityValidationException)e).EntityValidationErrors);
								Console.WriteLine(e.ToString());
								Console.WriteLine("Skipping priceBand.");
							}
						}
					}
				}
			}
		}
			
