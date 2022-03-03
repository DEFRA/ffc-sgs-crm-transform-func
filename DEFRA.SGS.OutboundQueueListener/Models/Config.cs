namespace DEFRA.SGS.Listener.Models
{
    public class Config
    {
        public string BaseAddress { get; set; }
        public string ClientId { get; set; }
        public string Secret { get; set; }
        public string TenantId { get; set; }
        public string Authority { get; set; }
        public string Api { get; set; }
        public int MaxRetries { get; set; }
        public bool IsLocalTest { get; set; }
    }
}
