using System;

namespace DEFRA.SGS.Listener.Models
{
    public class Organisation
    {
        public Guid OrganisationId { get; set; }
        public string Name { get; set; } = "";
        public string Email { get; set; } = "";
    }
}
