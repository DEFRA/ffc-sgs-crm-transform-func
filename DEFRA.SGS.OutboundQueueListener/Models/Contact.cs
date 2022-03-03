using System;

namespace DEFRA.SGS.Listener.Models
{
    public class Contact
    {
        public Guid ContactId { get; set; }
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Email { get; set; } = "";
    }
}
