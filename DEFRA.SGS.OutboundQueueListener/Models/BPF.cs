using System;

namespace DEFRA.SGS.Listener.Models
{
    public class BPF
    {
        public Guid BPFId { get; set; }
        public string Name { get; set; } = "";
        public Guid ActiveStage { get; set; }
    }
}
