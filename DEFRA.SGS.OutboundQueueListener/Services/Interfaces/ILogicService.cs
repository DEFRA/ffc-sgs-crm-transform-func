using System;
using DEFRA.SGS.Listener.Models.AgreementCalculator;
using Microsoft.Extensions.Logging;

namespace DEFRA.SGS.Listener.Services.Interfaces
{
    public interface ILogicService
    {
        void EligibilityCheckRequest(Guid caseId, ILogger log);
        void EligibilityCheckResponse(ResponsePayload response, ILogger log);
        void AgreementCalculatorRequest(Guid applicationId, ILogger log);
        void AgreementCalculatorResponse(ResponsePayload response, ILogger log);
        void OutboundListenerTest(Guid applicationId, ILogger log);
    }
}
