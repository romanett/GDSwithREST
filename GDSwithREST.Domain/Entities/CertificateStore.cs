namespace GDSwithREST.Domain.Entities
{
    public sealed record CertificateStore
    {
        public CertificateStore()
        {
            ApplicationsHttpsTrustList = new HashSet<Application>();
            ApplicationsTrustList = new HashSet<Application>();
        }

        public int Id { get; set; }
        public string Path { get; set; } = null!;
        public string AuthorityId { get; set; } = null!;

        public ICollection<Application> ApplicationsHttpsTrustList { get; set; }
        public ICollection<Application> ApplicationsTrustList { get; set; }
    }
}
