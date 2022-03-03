using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using DEFRA.SGS.Listener.Models;
using Microsoft.Extensions.Logging;

namespace DEFRA.SGS.Listener.Repositories.Interfaces
{
    public interface IDynamicsRepository : IDynamicsBaseRepository
    {
        Task<List<Application>> GetApplications(HttpClient client, ILogger log);
        Task<Application> GetApplication(HttpClient client, Guid applicationId, ILogger log);
        Task<Application> GetApplication(HttpClient client, string sbi, ILogger log);
        Task<bool> UpdateApplication(HttpClient client, Application application, ILogger log);
        Task<Case> GetCase(HttpClient client, Guid caseId, ILogger log);
        Task<Case> GetCase(HttpClient client, string sbi, ILogger log);
        Task<bool> UpdateCase(HttpClient client, Case grantCase, ILogger log);
    }
}
