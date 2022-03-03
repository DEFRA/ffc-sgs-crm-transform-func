using System;

namespace DEFRA.SGS.Listener.Models
{
    public class SLAInstance
    {
        public Guid SLAInstanceId { get; set; }
        public DateTime FailureTime { get; set; }
    }
}
