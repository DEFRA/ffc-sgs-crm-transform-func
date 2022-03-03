using Newtonsoft.Json;

namespace DEFRA.SGS.Listener.Models
{
    public class AgreementCalculatorRequest
    {
        [JsonProperty("uniqueReference", Required = Required.Default)]
        public string UniqueReference { get; set; }

        [JsonProperty("entityName", Required = Required.Default)]
        public string EntityName { get; set; }

        [JsonProperty("thirdPartyLogonId", Required = Required.Default)]
        public string ThirdPartyLogonId { get; set; }

        [JsonProperty("actionReference", Required = Required.Default)]
        public string ActionReference { get; set; }

        [JsonProperty("d365InstanceReference", Required = Required.Default)]
        public string D365InstanceReference { get; set; }
    }
}