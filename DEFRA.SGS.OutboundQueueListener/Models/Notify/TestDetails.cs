using Newtonsoft.Json;

namespace DEFRA.SGS.Listener.Models.Notify
{
    public class TestDetails
    {
        [JsonProperty("referenceNumber", Required = Required.Default)]
        public string ReferenceNumber { get; set; }

        [JsonProperty("firstName", Required = Required.Default)]
        public string FirstName { get; set; }

        [JsonProperty("lastName", Required = Required.Default)]
        public string LastName { get; set; }

        [JsonProperty("email", Required = Required.Default)]
        public string Email { get; set; }

        [JsonProperty("farmerName", Required = Required.Default)]
        public string FarmerName { get; set; }

        [JsonProperty("farmerSurname", Required = Required.Default)]
        public string FarmerSurname { get; set; }

        [JsonProperty("farmerEmail", Required = Required.Default)]
        public string FarmerEmail { get; set; }

        [JsonProperty("projectName", Required = Required.Default)]
        public string ProjectName { get; set; }

        [JsonProperty("businessName", Required = Required.Default)]
        public string BusinessName { get; set; }

        [JsonProperty("contactConsent", Required = Required.Default)]
        public string ContactConsent { get; set; }

        [JsonProperty("scoreDate", Required = Required.Default)]
        public string ScoreDate { get; set; }

        [JsonProperty("actionLink", Required = Required.Default)]
        public string ActionLink { get; set; }

        [JsonProperty("isProcessStage", Required = Required.Default)]
        public string IsProcessStage { get; set; }

        [JsonProperty("businessProcessName", Required = Required.Default)]
        public string BusinessProcessName { get; set; }

        [JsonProperty("stageName", Required = Required.Default)]
        public string StageName { get; set; }

        [JsonProperty("slaExpireOn", Required = Required.Default)]
        public string SlaExpireOn { get; set; }

        [JsonProperty("isSla", Required = Required.Default)]
        public string IsSla { get; set; }
    }
}
