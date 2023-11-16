namespace GDSwithREST.Domain.Entities
{
    public sealed record Application
    {
        public Application()
        {
            ApplicationNames = new HashSet<ApplicationName>();
            CertificateRequests = new HashSet<CertificateRequest>();
            ServerEndpoints = new HashSet<ServerEndpoint>();
        }

        public int Id { get; set; }
        public Guid ApplicationId { get; set; }
        public string ApplicationUri { get; set; } = null!;
        public string ApplicationName { get; set; } = null!;
        public int ApplicationType { get; set; }
        public string ProductUri { get; set; } = null!;
        public string ServerCapabilities { get; set; } = null!;
        public byte[] Certificate { get; set; } = Array.Empty<byte>();
        public byte[]? HttpsCertificate { get; set; }
        public int? TrustListId { get; set; }
        public int? HttpsTrustListId { get; set; }

        public CertificateStore HttpsTrustList { get; set; } = null!;
        public CertificateStore TrustList { get; set; } = null!;
        public ICollection<ApplicationName> ApplicationNames { get; set; }
        public ICollection<CertificateRequest> CertificateRequests { get; set; }
        public ICollection<ServerEndpoint> ServerEndpoints { get; set; }
    }
}
