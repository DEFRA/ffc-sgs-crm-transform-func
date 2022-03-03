using System;
using System.Net.Http;
using System.Threading.Tasks;
using DEFRA.SGS.Listener.Models;
using Microsoft.Extensions.Logging;

namespace DEFRA.SGS.Listener.Repositories.Interfaces
{
    public interface INotifyDynamicsRepository : IDynamicsBaseRepository
    {
        Task<Case> GetCase(HttpClient client, Guid caseId, ILogger log);
        Task<Application> GetApplication(HttpClient client, Guid applicationId, ILogger log);
        Task<Contact> GetContact(HttpClient client, Guid contactId, ILogger log);
        Task<Organisation> GetOrganisation(HttpClient client, Guid organisationId, ILogger log);
        Task<SLAInstance> GetSLAInstance(HttpClient client, Guid slaInstanceId, ILogger log);
        Task<BPF> GetBPF(HttpClient client, Guid caseId, ILogger log);
        Task<string> GetProcessName(HttpClient client, Guid stageId, ILogger log);
    }
}
