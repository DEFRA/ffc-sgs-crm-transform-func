using DEFRA.SGS.Listener.Models;
using DEFRA.SGS.Listener.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace DEFRA.SGS.Listener.Repositories
{
    internal class NotifyDynamicsRepository : DynamicsBaseRepository, INotifyDynamicsRepository
    {
        public NotifyDynamicsRepository(HttpClient httpClient, Config config) : base(httpClient, config) { }

        public async Task<Application> GetApplication(HttpClient client, Guid applicationId, ILogger log)
        {
            var query = "dff_grantapplications?fetchXml=" +
                        HttpUtility.UrlEncode(
                            $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                            "<entity name='dff_grantapplication'>" +
                                "<attribute name='dff_grantapplicationid'/>" +
                                "<attribute name='dff_applicantcontactid' />" +
                                "<attribute name='dff_applicaitonorganisationid' />" +
                                "<attribute name='dff_portalgrantid' />" +
                                "<filter type='and'>" +
                                    "<condition attribute='dff_grantapplicationid' operator='eq' value='{" + applicationId + "}' />" +
                                "</filter>" +
                            "</entity>" +
                            "</fetch>");

            dynamic collection = await Get(client, query, log);

            if (collection != null && collection.value != null && collection.value.Count > 0)
            {
                Application application = null;

                var applicationOrganisationId = collection.value[0]._dff_applicaitonorganisationid_value;
                var applicationContactId = collection.value[0]._dff_applicantcontactid_value;

                if (applicationId != null)
                {
                    application = new Application
                    {
                        ApplicationId = Guid.Parse(applicationId.ToString()),
                        ApplicationOrganisationId = Guid.Parse(applicationOrganisationId.ToString()),
                        ApplicationContactId = Guid.Parse(applicationContactId.ToString()),
                        Reference = collection.value[0].dff_portalgrantid
                    };
                }

                return application;
            }

            return null;

        }

        public async Task<Contact> GetContact(HttpClient client, Guid contactId, ILogger log)
        {
            var query = "contacts?fetchXml=" +
                        HttpUtility.UrlEncode(
                            $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                            "<entity name='contact'>" +
                                "<attribute name='contactid'/>" +
                                "<attribute name='firstname'/>" +
                                "<attribute name='lastname'/>" +
                                "<attribute name='emailaddress1'/>" +
                                "<filter type='and'>" +
                                    "<condition attribute='contactid' operator='eq' value='{" + contactId + "}' />" +
                                "</filter>" +
                            "</entity>" +
                            "</fetch>");

            dynamic collection = await Get(client, query, log);

            if (collection != null && collection.value != null && collection.value.Count > 0)
            {
                Contact contact = new Contact
                {
                    ContactId = Guid.Parse(collection.value[0].contactid.Value),
                    FirstName = collection.value[0].firstname,
                    LastName = collection.value[0].lastname,
                    Email = collection.value[0].emailaddress1
                };

                return contact;
            }

            return null;
        }

        public async Task<Organisation> GetOrganisation(HttpClient client, Guid organisationId, ILogger log)
        {
            var query = "accounts?fetchXml=" +
                        HttpUtility.UrlEncode(
                            $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                            "<entity name='account'>" +
                                "<attribute name='accountid'/>" +
                                "<attribute name='name'/>" +
                                "<attribute name='emailaddress1'/>" +
                                "<filter type='and'>" +
                                    "<condition attribute='accountid' operator='eq' value='{" + organisationId + "}' />" +
                                "</filter>" +
                            "</entity>" +
                            "</fetch>");

            dynamic collection = await Get(client, query, log);

            if (collection != null && collection.value != null && collection.value.Count > 0)
            {
                Organisation organisation = new Organisation
                {
                    OrganisationId = Guid.Parse(collection.value[0].accountid.Value),
                    Name = collection.value[0].name,
                    Email = collection.value[0].emailaddress1
                };

                return organisation;
            }

            return null;
        }

        public async Task<Case> GetCase(HttpClient client, Guid caseId, ILogger log)
        {
            var query = "dff_cases?fetchXml=" +
                        HttpUtility.UrlEncode(
                            $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                            "<entity name='dff_case'>" +
                                "<attribute name='dff_caseid'/>" +
                                "<attribute name='dff_organisationid'/>" +
                                "<attribute name='dff_contactid'/>" +
                                "<attribute name='dff_grantapplicationid'/>" +
                                "<attribute name='dff_firstresponsebykpiid'/>" +
                                "<filter type='and'>" +
                                    "<condition attribute='dff_caseid' operator='eq' value='{" + caseId + "}' />" +
                                "</filter>" +
                            "</entity>" +
                            "</fetch>");

            dynamic collection = await Get(client, query, log);

            if (collection != null && collection.value != null && collection.value.Count > 0)
            {
                Case grantCase = new Case
                {
                    CaseId = Guid.Parse(collection.value[0].dff_caseid.Value),
                    OrganisationId = collection.value[0]._dff_organisationid_value,
                    ContactId = collection.value[0]._dff_contactid_value,
                    ApplicationId = collection.value[0]._dff_grantapplicationid_value,
                    SLAInstanceId = collection.value[0]._dff_firstresponsebykpiid_value
                };

                return grantCase;
            }

            return null;
        }

        public async Task<SLAInstance> GetSLAInstance(HttpClient client, Guid slaInstanceId, ILogger log)
        {
            var query = "slakpiinstances?fetchXml=" +
                        HttpUtility.UrlEncode(
                            $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                            "<entity name='slakpiinstance'>" +
                                "<attribute name='slakpiinstanceid'/>" +
                                "<attribute name='failuretime'/>" +
                                "<filter type='and'>" +
                                    "<condition attribute='slakpiinstanceid' operator='eq' value='{" + slaInstanceId + "}' />" +
                                "</filter>" +
                            "</entity>" +
                            "</fetch>");

            dynamic collection = await Get(client, query, log);

            if (collection != null && collection.value != null && collection.value.Count > 0)
            {
                SLAInstance slaInstance = new SLAInstance
                {
                    SLAInstanceId = Guid.Parse(collection.value[0].dff_caseid.Value),
                    FailureTime = collection.value[0].failuretime
                };

                return slaInstance;
            }

            return null;
        }

        public async Task<BPF> GetBPF(HttpClient client, Guid caseId, ILogger log)
        {
            var query = "dff_casestrategicgrantses?fetchXml=" +
                        HttpUtility.UrlEncode(
                            $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                            "<entity name='dff_casestrategicgrants'>" +
                                "<attribute name='businessprocessflowinstanceid'/>" +
                                "<attribute name='bpf_dff_caseid'/>" +
                                "<attribute name='bpf_name'/>" +
                                "<attribute name='activestageid'/>" +
                                "<filter type='and'>" +
                                    "<condition attribute='bpf_dff_caseid' operator='eq' value='{" + caseId + "}' />" +
                                "</filter>" +
                            "</entity>" +
                            "</fetch>");

            dynamic collection = await Get(client, query, log);

            if (collection != null && collection.value != null && collection.value.Count > 0)
            {
                BPF bpf = new BPF
                {
                    BPFId = Guid.Parse(collection.value[0].businessprocessflowinstanceid.Value),
                    Name = collection.value[0].bpf_name,
                    ActiveStage = collection.value[0]._activestageid_value
                };

                return bpf;
            }

            return null;
        }

        public async Task<string> GetProcessName(HttpClient client, Guid stageId, ILogger log)
        {
            var query = "processstages?fetchXml=" +
                        HttpUtility.UrlEncode(
                            $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                            "<entity name='processstage'>" +
                                "<attribute name='stagename'/>" +
                                "<attribute name='processstageid'/>" +
                                "<filter type='and'>" +
                                    "<condition attribute='processstageid' operator='eq' value='{" + stageId + "}' />" +
                                "</filter>" +
                            "</entity>" +
                            "</fetch>");

            dynamic collection = await Get(client, query, log);

            if (collection != null && collection.value != null && collection.value.Count > 0)
            {
                return collection.value[0].stagename;
            }

            return null;
        }
    }
}
