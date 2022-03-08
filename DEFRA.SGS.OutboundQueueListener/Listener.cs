using DEFRA.SGS.Listener.Services.Interfaces;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using DEFRA.SGS.Listener.Models.AgreementCalculator;

namespace DEFRA.SGS.Listener
{
    public class Listener
    {
        private ILogicService _logicService;

        public Listener(ILogicService logicService)
        {
            _logicService = logicService;
        }

        [FunctionName("EligibilityCheckRequest")]
        public void EligibilityCheckRequestListener([ServiceBusTrigger("ffc-sgs-eligibility-check-request",
            Connection = "RootManageSharedAccessKey")] string myQueueItem, ILogger log)
        {
            try
            {
                log?.LogInformation($"{nameof(EligibilityCheckRequestListener)} - Function processing message: {myQueueItem}");

                var jObject = (JObject)JsonConvert.DeserializeObject(myQueueItem);
                var jValue = (JValue)jObject["PrimaryEntityId"];
                log?.LogInformation($"PrimaryEntityId: {jValue}");

                Guid caseId = Guid.Parse(jValue.ToString()); 
                
                log?.LogInformation($"{nameof(EligibilityCheckRequestListener)} - Service started");

                _logicService.EligibilityCheckRequest(caseId, log);

                log?.LogInformation($"{nameof(EligibilityCheckRequestListener)} - Service completed");
            }
            catch (JsonSerializationException jex)
            {
                log?.LogError(jex, $"{nameof(EligibilityCheckRequestListener)} - Invalid Start Session Request JSON: {jex.InnerException?.Message ?? jex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                string errorMessage = ex.InnerException?.Message ?? ex.Message;

                log?.LogError(ex, $"{nameof(EligibilityCheckRequestListener)} function execution failed | Error: {errorMessage}");
                throw;
            }
        }

        [FunctionName("AgreementCalculateRequest")]
        public void AgreementCalculateRequestListener([ServiceBusTrigger("ffc-sfi-agreement-calculate-request",
                Connection = "RootManageSharedAccessKey")] string myQueueItem, ILogger log)
        {
            try
            {
                log?.LogInformation($"ServiceBus queue trigger function {nameof(AgreementCalculateRequestListener)} processed message: {myQueueItem}");

                var jObject = (JObject)JsonConvert.DeserializeObject(myQueueItem);
                var jValue = (JValue)jObject["PrimaryEntityId"];
                log?.LogInformation($"PrimaryEntityId: {jValue}");

                Guid applicationId = Guid.Parse(jValue.ToString());

                log?.LogInformation($"{nameof(AgreementCalculateRequestListener)} - Service started");

                _logicService.AgreementCalculatorRequest(applicationId, log);

                log?.LogInformation($"{nameof(AgreementCalculateRequestListener)} - Service completed");
            }
            catch (JsonSerializationException jex)
            {
                log?.LogError(jex, $"{nameof(AgreementCalculateRequestListener)} - Invalid Start Session Request JSON: {jex.InnerException?.Message ?? jex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                string errorMessage = ex.InnerException?.Message ?? ex.Message;

                log?.LogError(ex, $"{nameof(AgreementCalculateRequestListener)} function execution failed | Error: {errorMessage}");
                throw;
            }
        }

        [FunctionName("EligibilityCheckResponse")]
        public void EligibilityCheckResponseListener([ServiceBusTrigger("ffc-sgs-eligibility-check-response",
            Connection = "RootManageSharedAccessKey", IsSessionsEnabled = false)] string myQueueItem, ILogger log)
        {
            log?.LogInformation($"ServiceBus queue trigger function {nameof(EligibilityCheckResponseListener)} processed message: {myQueueItem}");

            try
            {
                ResponsePayload response = JsonConvert.DeserializeObject<ResponsePayload>(myQueueItem);

                log?.LogInformation($"{nameof(EligibilityCheckResponseListener)} - Service started");

                _logicService.EligibilityCheckResponse(response, log);

                log?.LogInformation($"{nameof(EligibilityCheckResponseListener)} - Service completed");

            }
            catch (Exception ex)
            {
                string errorMessage = ex.InnerException?.Message ?? ex.Message;

                log?.LogError(ex, $"{nameof(EligibilityCheckResponseListener)} function execution failed | Error: {errorMessage}");
                throw;
            }
        }

        [FunctionName("AgreementCalculateResponse")]
        public void AgreementCalculateResponseListener([ServiceBusTrigger("ffc-sfi-agreement-calculate-response", 
            Connection = "RootManageSharedAccessKey", IsSessionsEnabled = true)] string myQueueItem, ILogger log)
        {
            log?.LogInformation($"ServiceBus queue trigger function {nameof(AgreementCalculateResponseListener)} processed message: {myQueueItem}");

            try
            {
                ResponsePayload response = JsonConvert.DeserializeObject<ResponsePayload>(myQueueItem);



            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
            }
        }

        [FunctionName("OutboundQueueListener")]
        public void RunOutboundQueueListener([ServiceBusTrigger("crm-outbound", Connection = "OutboundQueueConnectionString")] string myQueueItem, ILogger log)
        {
            log?.LogInformation($"ServiceBus queue trigger function {nameof(RunOutboundQueueListener)} processed message: {myQueueItem}");

            //var clientInbound = new ServiceBusClient(sbInboundConnectionString);
            //var senderInbound = clientInbound.CreateSender(sbInboundQueueName);

            try
            {
                var jobject = (JObject)JsonConvert.DeserializeObject(myQueueItem);
                var jvalue = (JValue)jobject["PrimaryEntityId"];
                log?.LogInformation($"PrimaryEntityId: {jvalue}");

                Guid applicationId = Guid.Parse(jvalue.ToString());

                _logicService.OutboundListenerTest(applicationId, log);
                //var dynamicsRepository = new DynamicsRepository();

                // write to Inbound queue
                //Task.RunOutboundQueueListener(async () => await SendToQueueAsync(senderInbound, message, messageBody));

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                //Console.ReadKey();

                //senderInbound.DisposeAsync();
                //clientInbound.DisposeAsync();
            }
        }

        //[FunctionName("InboundTopicListener")]
        //public void RunInboundTopicListener([ServiceBusTrigger("sgs-application-topic", "SGS-APPLICATION-CRM-SUBSCRIPTION", Connection = "InboundTopicConnectionString")] string myQueueItem,
        //    ILogger log)
        //{
        //    log?.LogInformation($"C# ServiceBus topic trigger function processed message: {myQueueItem}");

        //    //var clientInbound = new ServiceBusClient(sbInboundConnectionString);
        //    //var senderInbound = clientInbound.CreateSender(sbInboundQueueName);

        //    try
        //    {

        //        var jobject = (JObject)JsonConvert.DeserializeObject(myQueueItem);
        //        //var jvalue = (JValue)jobject["PrimaryEntityId"];
        //        //log?.LogInformation($"PrimaryEntityId: {jvalue}");

        //        //var dynamicsRepository = new DynamicsRepository();

        //        // write to Inbound queue
        //        //Task.RunOutboundQueueListener(async () => await SendToQueueAsync(senderInbound, message, messageBody));

        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //    finally
        //    {
        //        //Console.ReadKey();

        //        //senderInbound.DisposeAsync();
        //        //clientInbound.DisposeAsync();
        //    }
        //}

    }
}
