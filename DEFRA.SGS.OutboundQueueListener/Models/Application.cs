using System;

namespace DEFRA.SGS.Listener.Models
{
    public class Application
    {
        public Guid ApplicationId { get; set; }
        public string Reference { get; set; } = "";
        public string Timestamp { get; set; } = "";
        public string SBI { get; set; } = "";
        public string SingleBusinessIdentifier { get; set; } = "";
        public string UniqueOrganisationIdentifier { get; set; } = "";
        public string OrganisationName { get; set; } = "";
        public string OrganisationAddress { get; set; } = "";
        public Guid? ApplicationContactId { get; set; }
        public Guid? ApplicationOrganisationId { get; set; }

    }
}
