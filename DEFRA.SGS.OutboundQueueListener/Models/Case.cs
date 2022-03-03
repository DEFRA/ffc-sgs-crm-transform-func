using System;

namespace DEFRA.SGS.Listener.Models
{
    public class Case
    {
        public Guid CaseId { get; set; }
        public int CallerID { get; set; }
        public int CustomerReferenceNumber { get; set; }
        public string Timestamp { get; set; } = "";
        public string SBI { get; set; } = "";
        public string SingleBusinessIdentifier { get; set; } = "";
        public string UniqueOrganisationIdentifier { get; set; } = "";
        public string OrganisationName { get; set; } = "";
        public string OrganisationAddress { get; set; } = "";
        public Guid? ContactId { get; set; }
        public Guid? OrganisationId { get; set; }
        public Guid? ApplicationId { get; set; }
        public Application? Application { get; set; }
        public Guid? SLAInstanceId { get; set; }
    }
}
