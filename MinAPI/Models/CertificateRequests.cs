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
        public string CertificateGroupId { get; set; }
        public string CertificateTypeId { get; set; }
        public byte[] CertificateSigningRequest { get; set; }
        public string SubjectName { get; set; }
        public string DomainNames { get; set; }
        public string PrivateKeyFormat { get; set; }
        public string PrivateKeyPassword { get; set; }
        public string AuthorityId { get; set; }

        public Applications Application { get; set; }
    }
}
