using DEFRA.SGS.Listener.Models.Notify;
using Microsoft.Extensions.Logging;

namespace DEFRA.SGS.Listener.Services.Interfaces
{
    public interface INotifyLogicService
    {
        void NotifyRequest(NotifyContext notifyContext, ILogger log);
    }
}
