using DEFRA.SGS.Listener.Models;
using DEFRA.SGS.Listener.Repositories.Interfaces;
using DEFRA.SGS.Listener.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using DEFRA.SGS.Listener.Models.AgreementCalculator;
using Microsoft.Extensions.Azure;
using Newtonsoft.Json;

namespace DEFRA.SGS.Listener.Services
{
    public class LogicService : ILogicService
    {
        private IDynamicsRepository _dynamicsRepository;

        public LogicService(IDynamicsRepository dynamicsRepository)
        {
            _dynamicsRepository = dynamicsRepository;
        }

        public async void EligibilityCheckRequest(Guid caseId, ILogger log)
        {
            var grantCase = await GetCase(caseId, log);

            // build payload and send to 'ffc-sfi-eligibility-check' topic
            var requestMessage = new RequestPayload
            {
                CustomerReferenceNumber = grantCase.CustomerReferenceNumber,
                CallerId = grantCase.CallerID
            };

            var connectionString = Environment.GetEnvironmentVariable("RootManageSharedAccessKey");
            var topic = Environment.GetEnvironmentVariable("EligibilityRequestTopic");

            await SendToTopicAsync(requestMessage, connectionString, topic);
        }

        public async void EligibilityCheckResponse(ResponsePayload response, ILogger log)
        {
            if (response != null)
            {
                if (response.Eligibility != null)
                {
                    var repoClient = await _dynamicsRepository.GetConnection(log);

                    for (int n = 0; n < response.Eligibility.Length; n++)
                    {
                        if (response.Eligibility[n].SingleBusinessIdentifier > 0)
                        {
                            var grantCase = await _dynamicsRepository.GetCase(repoClient,
                                response.Eligibility[n].SingleBusinessIdentifier.ToString(), log);

                            if (grantCase != null && grantCase.CaseId != Guid.Empty)
                            {
                                grantCase.Timestamp = DateTime.Now.ToLongTimeString();
                                grantCase.SingleBusinessIdentifier = response.Eligibility[n].SingleBusinessIdentifier > 0 ?
                                    response.Eligibility[n].SingleBusinessIdentifier.ToString() : null;
                                grantCase.UniqueOrganisationIdentifier = response.Eligibility[n].UniqueOrganisationIdentifier > 0 ?
                                    response.Eligibility[n].UniqueOrganisationIdentifier.ToString() : null;
                                grantCase.OrganisationName = response.Eligibility[n].OrganisationName;
                                grantCase.OrganisationAddress = response.Eligibility[n].OrganisationAddress;

                                await _dynamicsRepository.UpdateCase(repoClient, grantCase, log);
                            }
                        }
                    }
                }
            }
        }

        public async void AgreementCalculatorRequest(Guid applicationId, ILogger log)
        {
            // test values
            const int crn = 1101001509;
            const int callerId = 5100150;

             // build payload and send to 'ffc-sfi-agreement-calculate' topic
            var requestMessage = new RequestPayload
            {
                CustomerReferenceNumber = crn,
                CallerId = callerId
            };

            var connectionString = Environment.GetEnvironmentVariable("RootManageSharedAccessKey");
            var topic = Environment.GetEnvironmentVariable("AgreementCalculateRequestTopic");

            await SendToTopicAsync(requestMessage, connectionString, topic);
        }

        public async void AgreementCalculatorResponse(ResponsePayload response, ILogger log)
        {

        }

        public async void OutboundListenerTest(Guid applicationId, ILogger log)
        {
            var application = await GetApplication(applicationId, log);

            application.Timestamp = DateTime.Now.ToLongTimeString();

            var client = await _dynamicsRepository.GetConnection(log);

            await _dynamicsRepository.UpdateApplication(client, application, log);
        }

        private async Task SendToTopicAsync(RequestPayload requestMessage, string connectionString, string topic)
        {
            var messageBody = JsonConvert.SerializeObject(requestMessage);

            var client = new ServiceBusClient(connectionString);
            var sender = client.CreateSender(topic);
            var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(messageBody));

            await sender.SendMessageAsync(message);

            Console.WriteLine($"SendToTopicAsync: Message Added in Topic: {messageBody}");
        }

        private async Task<Application> GetApplication(Guid applicationId, ILogger log)
        {
            var client = await _dynamicsRepository.GetConnection(log);

            var application = await _dynamicsRepository.GetApplication(client, applicationId, log);

            return application;
        }

        private async Task<Case> GetCase(Guid caseId, ILogger log)
        {
            var client = await _dynamicsRepository.GetConnection(log);

            var grantCase = await _dynamicsRepository.GetCase(client, caseId, log);

            return grantCase;
        }
    }
}
