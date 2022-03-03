using Newtonsoft.Json;

namespace DEFRA.SGS.Listener.Models.AgreementCalculator
{
    public class ResponsePayload
    {
        [JsonProperty("eligibility", Required = Required.Default)]
        public EligibiltyResponse[] Eligibility { get; set; }
    }

    public class EligibiltyResponse
    {
        [JsonProperty("sbi", Required = Required.Default)]
        public int SingleBusinessIdentifier { get; set; }

        [JsonProperty("name", Required = Required.Default)]
        public string OrganisationName { get; set; }

        [JsonProperty("organisationId", Required = Required.Default)]
        public int UniqueOrganisationIdentifier { get; set; }

        [JsonProperty("address", Required = Required.Default)]
        public string OrganisationAddress { get; set; }
    }
}
