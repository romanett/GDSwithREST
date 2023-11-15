namespace GDSwithREST.Domain.Entities
{
    public sealed class CertificateRequest
    {
        public int Id { get; set; }
        public Guid RequestId { get; set; }
        public int? ApplicationId { get; set; }
        public int State { get; set; }
        public string CertificateGroupId { get; set; } = null!;
        public string CertificateTypeId { get; set; } = null!;
        public byte[]? CertificateSigningRequest { get; set; }
        public string? SubjectName { get; set; } = null!;
        public string? DomainNames { get; set; } = null!;
        public string? PrivateKeyFormat { get; set; } = null!;
        public string? PrivateKeyPassword { get; set; } = null!;
        public string AuthorityId { get; set; } = null!;

        public Application Application { get; set; } = null!;
    }
}
