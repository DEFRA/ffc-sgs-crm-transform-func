using System;

namespace DEFRA.SGS.Listener.Models.Notify
{
    public class NotifyContext
    {
        public Guid EntityId { get; set; }
        public string EntityName { get; set; }
        public string TemplateID { get; set; }
        public string CalledFrom { get; set; }
    }
}
