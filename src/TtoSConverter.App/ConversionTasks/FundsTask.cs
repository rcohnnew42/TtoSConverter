using System;
using System.Linq;
using Ambitus.Rest.Operations;
using Intermediates;
namespace TtoSConverter.App.ConversionTasks
{
	public class FundsTask : IConversionTask
	{
		private readonly Ambitus.Rest.IApiClient restClient;
		private readonly Ambitus.Soap.IApiClient soapClient;

		public FundsTask(Ambitus.Rest.IApiClient restClient, Ambitus.Soap.IApiClient soapClient)
		{
			this.restClient = restClient;
			this.soapClient = soapClient;
		}

		public void Execute()
		{
			using (var context = new IntermediatesEntities())
			{

				
						var fundsResult = restClient.Send(new GetFundSummariesQuery());
						var existingFunds = context.Funds.Select(fund => fund.FundId);

						var funds = fundsResult.Data;

						foreach (var fund in funds)
						{
							try
							{
								var fundId = fund.Id.Value;
								var fundDescription = fund.Description;

								if (existingFunds.Contains(fundId.ToString()))
								{
									Console.WriteLine("Skipping Created Fund {0}", fundId);
									continue;
								}



								var newFund = new Fund
								{
									FundId = fundId.ToString(),
									Name = fundDescription
								};

								Console.WriteLine("Adding fund {0}", fundId);
								context.Funds.Add(newFund);
								context.SaveChanges();
							}
							catch (Exception e)
							{
								Console.WriteLine("An exception occurred:");
								Console.WriteLine(((System.Data.Entity.Validation.DbEntityValidationException)e).EntityValidationErrors);
								Console.WriteLine(e.ToString());
								Console.WriteLine("Skipping fund.");
							}
						}
					}
				}
			}
		}
			
