namespace GDSwithREST.Domain.Entities
{
    public sealed record TrustList
    {
        public int Id { get; set; }
        public int ApplicationId { get; set; }
        public  string Path { get; set; } = null!;
        public  string CertificateType { get; set; } = null!;
        public  Application Application { get; set; } = null!;
    }
}