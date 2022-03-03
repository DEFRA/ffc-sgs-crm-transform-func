using Newtonsoft.Json;

namespace DEFRA.SGS.Listener.Models.AgreementCalculator
{
    public class RequestMessage
    {
        [JsonProperty("headers", Required = Required.Default)]
        public RequestHeader Header { get; set; }

        [JsonProperty("payload", Required = Required.Default)]
        public RequestPayload Payload { get; set; }
    }
}
