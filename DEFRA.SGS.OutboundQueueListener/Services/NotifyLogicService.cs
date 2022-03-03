using DEFRA.SGS.Listener.Models;
using DEFRA.SGS.Listener.Repositories.Interfaces;
using DEFRA.SGS.Listener.Services.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using DEFRA.SGS.Listener.Models.Notify;
using Newtonsoft.Json;

namespace DEFRA.SGS.Listener.Services
{
    public class NotifyLogicService : INotifyLogicService
    {
        private INotifyDynamicsRepository _dynamicsRepository;

        private const string TemplateTypeTest = "8a27798d-b2d6-4a58-b719-78937fe81024";
        private const string CaseEntity = "dff_case";

        public NotifyLogicService(INotifyDynamicsRepository dynamicsRepository)
        {
            _dynamicsRepository = dynamicsRepository;
        }

        public async void NotifyRequest(NotifyContext notifyContext, ILogger log)
        {
            if (notifyContext.TemplateID == TemplateTypeTest && notifyContext.EntityName == CaseEntity)
            {
                DoTestNotify(notifyContext,log);
            }
        }

        private async void DoTestNotify(NotifyContext notifyContext, ILogger log)
        {
            var grantCase = await GetNotifyCase(notifyContext.EntityId, log);

            if (grantCase != null)
            {

                var notifyTest = new Test
                {
                    Details = new TestDetails(),
                    NotifyTemplate = notifyContext.TemplateID,
                };

                // TODO: check if we can remove any of these hard-coded values
                notifyTest.Details.ActionLink = "http://localhost:3800/applicationstages/SGS001-5B6-2E8/undefined/response";
                notifyTest.Details.IsProcessStage = "No";
                notifyTest.Details.IsSla = "No";
                notifyTest.Details.ContactConsent = "No";       // TODO: no mapping specified yet
                notifyTest.Details.ScoreDate = string.Empty;    // TODO: no mapping specified yet

                var bpf = await GetCaseBPF(notifyContext.EntityId, log);

                if (bpf != null)
                {
                    notifyTest.Details.BusinessProcessName = bpf.Name;
                    notifyTest.Details.StageName = await GetCurrentStage(bpf.ActiveStage, log);
                }

                if (grantCase.ContactId != null)
                {
                    var caseContact = await GetNotifyContact(grantCase.ContactId.Value, log);

                    if (caseContact != null)
                    {
                        notifyTest.EmailAddress = caseContact.Email;
                        notifyTest.Details.FirstName = caseContact.FirstName;
                        notifyTest.Details.LastName = caseContact.LastName;
                        notifyTest.Details.Email = caseContact.Email;
                    }
                }

                if (grantCase.OrganisationId != null)
                {
                    var caseOrganisation = await GetNotifyOrganisation(grantCase.OrganisationId.Value, log);

                    if (caseOrganisation != null)
                    {
                        // TODO: is this section required?
                    }
                }

                notifyTest.Details.SlaExpireOn = string.Empty;
                if (grantCase.SLAInstanceId != null)
                {
                    var slaInstance = await GetNotifySLAInstance(grantCase.SLAInstanceId.Value, log);

                    if (slaInstance != null)
                    {
                        notifyTest.Details.SlaExpireOn = slaInstance.FailureTime.ToString();
                    }
                }

                if (grantCase.ApplicationId != null)
                {
                    var application = await GetNotifyApplication(grantCase.ApplicationId.Value, log);

                    if (application != null)
                    {
                        notifyTest.Reference = application.Reference;
                        notifyTest.Details.ReferenceNumber = application.Reference;

                        if (application.ApplicationContactId != null)
                        {
                            var applicationContact = await GetNotifyContact(application.ApplicationContactId.Value, log);

                            if (applicationContact != null)
                            {
                                notifyTest.Details.FarmerName = applicationContact.FirstName;
                                notifyTest.Details.FarmerSurname = applicationContact.LastName;
                                notifyTest.Details.FarmerEmail = applicationContact.Email;
                            }
                        }

                        if (application.ApplicationOrganisationId != null)
                        {
                            var applicationOrganisation = await GetNotifyOrganisation(application.ApplicationOrganisationId.Value, log);

                            if (applicationOrganisation != null)
                            {
                                notifyTest.Details.BusinessName = applicationOrganisation.Name;
                                notifyTest.Details.ProjectName = applicationOrganisation.Name;
                            }
                        }
                    }
                }

                string messageBody = JsonConvert.SerializeObject(notifyTest);

                var connectionString = Environment.GetEnvironmentVariable("RootManageSharedAccessKey");
                var queue = Environment.GetEnvironmentVariable("NotificationRequestQueue");

                await SendToQueueAsync(messageBody, connectionString, queue);
            }
        }

        private async Task<string> GetCurrentStage(Guid stageId, ILogger log)
        {
            var client = await _dynamicsRepository.GetConnection(log);

            var stageName = await _dynamicsRepository.GetProcessName(client, stageId, log);

            return stageName;
        }

        private async Task<BPF> GetCaseBPF(Guid caseId, ILogger log)
        {
            var client = await _dynamicsRepository.GetConnection(log);

            var bpf = await _dynamicsRepository.GetBPF(client, caseId, log);

            return bpf;
        }

        private async Task<Case> GetNotifyCase(Guid caseId, ILogger log)
        {
            var client = await _dynamicsRepository.GetConnection(log);

            var grantCase = await _dynamicsRepository.GetCase(client, caseId, log);

            return grantCase;
        }

        private async Task<Application> GetNotifyApplication(Guid applicationId, ILogger log)
        {
            var client = await _dynamicsRepository.GetConnection(log);

            var application = await _dynamicsRepository.GetApplication(client, applicationId, log);

            return application;
        }

        private async Task<Contact> GetNotifyContact(Guid contactId, ILogger log)
        {
            var client = await _dynamicsRepository.GetConnection(log);

            var contact = await _dynamicsRepository.GetContact(client, contactId, log);

            return contact;
        }

        private async Task<Organisation> GetNotifyOrganisation(Guid organisationId, ILogger log)
        {
            var client = await _dynamicsRepository.GetConnection(log);

            var organisation = await _dynamicsRepository.GetOrganisation(client, organisationId, log);

            return organisation;
        }

        private async Task<SLAInstance> GetNotifySLAInstance(Guid slaInstanceId, ILogger log)
        {
            var client = await _dynamicsRepository.GetConnection(log);

            var slaInstance = await _dynamicsRepository.GetSLAInstance(client, slaInstanceId, log);

            return slaInstance;
        }

        private async Task SendToQueueAsync(string messageBody, string connectionString, string queue)
        {
            var client = new ServiceBusClient(connectionString);
            var sender = client.CreateSender(queue);
            var message = new ServiceBusMessage(Encoding.UTF8.GetBytes(messageBody));

            await sender.SendMessageAsync(message);

            Console.WriteLine($"SendToQueueAsync: Message Added in Queue: {messageBody}");
        }
    }
}
