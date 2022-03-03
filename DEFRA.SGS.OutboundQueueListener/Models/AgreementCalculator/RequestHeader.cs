using Newtonsoft.Json;

namespace DEFRA.SGS.Listener.Models.AgreementCalculator
{
    public class RequestHeader
    {
        [JsonProperty("correlationId", Required = Required.Default)]
        public string CorrelationId { get; set; }

        [JsonProperty("messageId", Required = Required.Default)]
        public string MessageId { get; set; }
    }
}
