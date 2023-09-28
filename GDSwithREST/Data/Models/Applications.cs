namespace GDSwithREST.Data.Models
{
    public partial class Applications
    {
        public Applications()
        {
            ApplicationNames = new HashSet<ApplicationNames>();
            CertificateRequests = new HashSet<CertificateRequests>();
            ServerEndpoints = new HashSet<ServerEndpoints>();
        }

        public int Id { get; set; }
        public Guid ApplicationId { get; set; }
        public string ApplicationUri { get; set; } = null!;
        public string ApplicationName { get; set; } = null!;
        public int ApplicationType { get; set; }
        public string ProductUri { get; set; } = null!;
        public string ServerCapabilities { get; set; } = null!;
        public byte[]? Certificate { get; set; }
        public byte[]? HttpsCertificate { get; set; }
        public int? TrustListId { get; set; }
        public int? HttpsTrustListId { get; set; }

        public CertificateStores HttpsTrustList { get; set; } = null!;
        public CertificateStores TrustList { get; set; } = null!;
        public ICollection<ApplicationNames> ApplicationNames { get; set; }
        public ICollection<CertificateRequests> CertificateRequests { get; set; }
        public ICollection<ServerEndpoints> ServerEndpoints { get; set; }
    }
}
