using Newtonsoft.Json;

namespace DEFRA.SGS.Listener.Models.AgreementCalculator
{
    public class RequestPayload
    {
        [JsonProperty("crn", Required = Required.Default)]
        public int CustomerReferenceNumber { get; set; }

        [JsonProperty("callerId", Required = Required.Default)]
        public int CallerId { get; set; }
    }
}