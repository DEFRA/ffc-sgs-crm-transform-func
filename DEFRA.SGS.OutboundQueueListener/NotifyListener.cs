using DEFRA.SGS.Listener.Services.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Runtime.Serialization.Json;
using DEFRA.SGS.Listener.Models;
using DEFRA.SGS.Listener.Models.Notify;
using Newtonsoft.Json.Serialization;

namespace DEFRA.SGS.Listener
{
    public class NotifyListener
    {
        private INotifyLogicService _logicService;

        public NotifyListener(INotifyLogicService logicService)
        {
            _logicService = logicService;
        }

        [FunctionName("NotifyRequest")]
        public void NotifyRequestListener([ServiceBusTrigger("ffc-sgs-notification-queue-request",
            Connection = "RootManageSharedAccessKey")] string myQueueItem, ILogger log)
        {
            try
            {
                log?.LogInformation($"{nameof(NotifyRequestListener)} - Function processing message: {myQueueItem}");

                var jObject = (JObject)JsonConvert.DeserializeObject(myQueueItem);

                var jPrimaryEntityId = (JValue)jObject["PrimaryEntityId"];
                log?.LogInformation($"PrimaryEntityId: {jPrimaryEntityId}");

                var jPrimaryEntityName = (JValue)jObject["PrimaryEntityName"];
                log?.LogInformation($"PrimaryEntityName: {jPrimaryEntityName}");

                var jPrimaryInputParameters = (JArray)jObject["InputParameters"];
                log?.LogInformation($"InputParameters: {jPrimaryInputParameters}");

                NotifyContext notifyContext = new NotifyContext();

                foreach (var jt in jPrimaryInputParameters)
                {
                    var inputParameter = JsonConvert.DeserializeObject<InputParameter>(jt.ToString(), new JsonSerializerSettings
                    {
                        Error = HandleDeserializationError
                    });

                    if (inputParameter != null)
                    {
                        if (inputParameter.key == "TemplateID")
                            notifyContext.TemplateID = inputParameter.value;

                        if (inputParameter.key == "CalledFrom")
                            notifyContext.CalledFrom = inputParameter.value;
                    }
                }

                notifyContext.EntityId = Guid.Parse(jPrimaryEntityId.ToString());
                notifyContext.EntityName = jPrimaryEntityName.ToString();

                log?.LogInformation($"{nameof(NotifyRequestListener)} - Service started");

                _logicService.NotifyRequest(notifyContext, log);

                log?.LogInformation($"{nameof(NotifyRequestListener)} - Service completed");

            }
            catch (JsonSerializationException jex)
            {
                log?.LogError(jex, $"{nameof(NotifyRequestListener)} - Invalid Start Session Request JSON: {jex.InnerException?.Message ?? jex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                string errorMessage = ex.InnerException?.Message ?? ex.Message;

                log?.LogError(ex, $"{nameof(NotifyRequestListener)} function execution failed | Error: {errorMessage}");
                throw;
            }
        }
        
        public void HandleDeserializationError(object sender, ErrorEventArgs errorArgs)
        {
            var currentError = errorArgs.ErrorContext.Error.Message;
            errorArgs.ErrorContext.Handled = true;
        }
    }
}
