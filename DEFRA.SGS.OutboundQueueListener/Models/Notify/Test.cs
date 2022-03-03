using Newtonsoft.Json;

namespace DEFRA.SGS.Listener.Models.Notify
{
    public class Test
    {
        [JsonProperty("ref", Required = Required.Default)]
        public string Reference { get; set; }

        [JsonProperty("notifyTemplate", Required = Required.Default)]
        public string NotifyTemplate { get; set; }

        [JsonProperty("emailAddress", Required = Required.Default)]
        public string EmailAddress { get; set; }

        [JsonProperty("details", Required = Required.Default)]
        public TestDetails Details { get; set; }
    }
}
