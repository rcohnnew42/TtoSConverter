using System;
using System.Linq;
using Ambitus.Soap.Methods.Inventory.GetHoldCodes;
using Ambitus.Soap.Methods.Session.CreateSession;
using Intermediates;
namespace TtoSConverter.ConversionTasks
{
	public class LockTypesTask : IConversionTask
	{
		private readonly Ambitus.Rest.IApiClient restClient;
		private readonly Ambitus.Soap.IApiClient soapClient;

		public LockTypesTask(Ambitus.Rest.IApiClient restClient, Ambitus.Soap.IApiClient soapClient)
		{
			this.restClient = restClient;
			this.soapClient = soapClient;
		}

		public void Execute()
		{
			const string CLIENT_USER_NAME = "rcohn";
			const string CLIENT_USER_PASSWORD = "Scurvy!1";
			const int BUSINESS_UNIT_ID = 1;

			using (var context = new IntermediatesEntities())
			{
						var sessionKey = soapClient.Send(new CreateSessionCommand(BUSINESS_UNIT_ID));
						var lockTypesResult = soapClient.Send(new GetHoldCodesQuery(sessionKey, CLIENT_USER_NAME, CLIENT_USER_PASSWORD));
						var existingLockTypes = context.LockTypes.Select(lt => lt.LockTypeId);
						foreach (var lockType in lockTypesResult)
						{
							try
							{
								var lockTypeId = lockType.Id.Value;
								var lockTypeName = lockType.Name;

								if (existingLockTypes.Contains(lockTypeId.ToString()))
								{
									Console.WriteLine("Skipping Created LockType {0}", lockTypeId);
									continue;
								}



								var newLockType = new LockType
								{
									LockTypeId = lockTypeId.ToString(),
									Name = lockTypeName
								};

								Console.WriteLine("Adding lockType {0}", lockTypeId);
								context.LockTypes.Add(newLockType);
								context.SaveChanges();
							}
							catch (Exception e)
							{
								Console.WriteLine("An exception occurred:");
								Console.WriteLine(((System.Data.Entity.Validation.DbEntityValidationException)e).EntityValidationErrors);
								Console.WriteLine(e.ToString());
								Console.WriteLine("Skipping lockType.");
							}
						}
					}
				}
			}
		}
			
