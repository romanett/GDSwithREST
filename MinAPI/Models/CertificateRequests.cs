using System;
using System.Collections.Generic;

namespace MinAPI.Models
{
    public partial class CertificateRequests
    {
        public int Id { get; set; }
        public Guid RequestId { get; set; }
        public int ApplicationId { get; set; }
        public int State { get; set; }
        public string CertificateGroupId { get; set; } = null!;
        public string CertificateTypeId { get; set; } = null!;
        public byte[] CertificateSigningRequest { get; set; } = null!;
        public string SubjectName { get; set; } = null!;
        public string DomainNames { get; set; } = null!;
        public string PrivateKeyFormat { get; set; } = null!;
        public string PrivateKeyPassword { get; set; } = null!;
        public string AuthorityId { get; set; } = null!;

        public Applications Application { get; set; } = null!;
    }
}
