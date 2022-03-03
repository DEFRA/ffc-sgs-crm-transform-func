using DEFRA.SGS.Listener.Models;
using DEFRA.SGS.Listener.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace DEFRA.SGS.Listener.Repositories
{
    public class DynamicsRepository : DynamicsBaseRepository, IDynamicsRepository
    {

        public DynamicsRepository(HttpClient httpClient, Config config) : base(httpClient, config) { }

        public async Task<List<Application>> GetApplications(HttpClient client, ILogger log)
        {
            var query = "dff_grantapplications?fetchXml=" +
                        HttpUtility.UrlEncode(
                            $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                            "<entity name='dff_grantapplication'>" +
                            "<attribute name='dff_grantapplicationid'/>" +
                            "</entity>" +
                            "</fetch>");

            var collection = await Get(client, query, log);

            List<Application> applications = new List<Application>();

            if (collection != null)
            {
                IEnumerable<dynamic> applicationList = collection.value;

                foreach (var app in applicationList)
                {

                    applications.Add(new Application
                    {
                        ApplicationId = app.dff_grantapplicationid != null
                            ? Guid.Parse(app.dff_grantapplicationid.Value)
                            : Guid.Empty
                    });
                }
            }

            return applications;
        }

        public async Task<Application> GetApplication(HttpClient client, Guid applicationId, ILogger log)
        {
            var query = "dff_grantapplications?fetchXml=" +
                        HttpUtility.UrlEncode(
                            $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                            "<entity name='dff_grantapplication'>" +
                                "<attribute name='dff_grantapplicationid'/>" +
                                "<filter type='and'>" +
                                    "<condition attribute='dff_grantapplicationid' operator='eq' value='{" + applicationId + "}' />" +
                                "</filter>" +
                            "</entity>" +
                            "</fetch>");

            dynamic collection = await Get(client, query, log);

            Application application = new Application
            {
                ApplicationId = collection != null && collection.value.Count > 0 ? Guid.Parse(collection.value[0].dff_grantapplicationid.Value) : Guid.Empty
            };

            return application;
        }

        public async Task<Application> GetApplication(HttpClient client, string sbi, ILogger log)
        {
            var query = "dff_grantapplications?fetchXml=" +
                        HttpUtility.UrlEncode(
                            $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                            "<entity name='dff_grantapplication'>" +
                                "<attribute name='dff_grantapplicationid'/>" +
                                "<attribute name='dff_sbi'/>" +
                                "<attribute name='dff_singlebusinessidentifier'/>" +
                                "<attribute name='dff_uniqueorganisationidentifier'/>" +
                                "<attribute name='dff_organisationname'/>" +
                                "<attribute name='dff_organisationaddress'/>" +
                                "<filter type='and'>" +
                                    "<condition attribute='dff_sbi' operator='eq' value='" + sbi + "' />" +
                                "</filter>" +
                            "</entity>" +
                            "</fetch>");

            dynamic collection = await Get(client, query, log);

            Application application = new Application
            {
                ApplicationId = collection != null && collection.value.Count > 0 ? Guid.Parse(collection.value[0].dff_grantapplicationid.Value) : Guid.Empty
            };

            return application;
        }

        public async Task<bool> UpdateApplication(HttpClient client, Application application, ILogger log)
        {
            JObject payload = new JObject{
                { "dff_timestamp", application.Timestamp },
                { "dff_singlebusinessidentifier", application.SingleBusinessIdentifier },
                { "dff_uniqueorganisationidentifier", application.UniqueOrganisationIdentifier },
                { "dff_organisationname", application.OrganisationName },
                { "dff_organisationaddress", application.OrganisationAddress }
            };

            const string columns = "dff_grantapplicationid,dff_timestamp,dff_singlebusinessidentifier,dff_uniqueorganisationidentifier,dff_organisationname,dff_organisationaddress";

            string query = $"dff_grantapplications({application.ApplicationId})?$select={columns}";

            dynamic data = await Patch(client, query, payload.ToString(), log);

            return data.dff_grantapplicationid != null;
        }

        public async Task<Case> GetCase(HttpClient client, Guid caseId, ILogger log)
        {
            var query = "dff_cases?fetchXml=" +
                        HttpUtility.UrlEncode(
                            $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                            "<entity name='dff_case'>" +
                                "<attribute name='dff_caseid'/>" +
                                "<attribute name='dff_customerreferencenumber'/>" +
                                "<attribute name='dff_callerid'/>" +
                                "<filter type='and'>" +
                                    "<condition attribute='dff_caseid' operator='eq' value='{" + caseId + "}' />" +
                                "</filter>" +
                            "</entity>" +
                            "</fetch>");

            dynamic collection = await Get(client, query, log);

            Case grantCase = new Case
            {
                CaseId = collection != null && collection.value.Count > 0 ? Guid.Parse(collection.value[0].dff_caseid.Value) : Guid.Empty,
                CustomerReferenceNumber = collection != null && collection.value.Count > 0 ? collection.value[0].dff_customerreferencenumber : 0,
                CallerID = collection != null && collection.value.Count > 0 ? collection.value[0].dff_callerid : 0
            };

            return grantCase;
        }

        public async Task<Case> GetCase(HttpClient client, string sbi, ILogger log)
        {
            var query = "dff_cases?fetchXml=" +
                        HttpUtility.UrlEncode(
                            $@"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>" +
                            "<entity name='dff_case'>" +
                                "<attribute name='dff_caseid'/>" +
                                "<filter type='and'>" +
                                    "<condition attribute='dff_sbi' operator='eq' value='" + sbi + "' />" +
                                "</filter>" +
                            "</entity>" +
                            "</fetch>");

            dynamic collection = await Get(client, query, log);

            Case grantCase = new Case
            {
                CaseId = collection != null && collection.value.Count > 0 ? Guid.Parse(collection.value[0].dff_caseid.Value) : Guid.Empty
            };

            return grantCase;
        }
 
        public async Task<bool> UpdateCase(HttpClient client, Case grantCase, ILogger log)
        {
            JObject payload = new JObject{
                { "dff_timestamp", grantCase.Timestamp },
                { "dff_singlebusinessidentifier", grantCase.SingleBusinessIdentifier },
                { "dff_uniqueorganisationidentifier", grantCase.UniqueOrganisationIdentifier },
                { "dff_organisationname", grantCase.OrganisationName },
                { "dff_organisationaddress", grantCase.OrganisationAddress }
            };

            const string columns = "dff_caseid,dff_timestamp,dff_singlebusinessidentifier,dff_uniqueorganisationidentifier,dff_organisationname,dff_organisationaddress";

            string query = $"dff_cases({grantCase.CaseId})?$select={columns}";

            dynamic data = await Patch(client, query, payload.ToString(), log);

            return data.dff_caseid != null;
        }
    }
}
