using System;
using System.Collections.Generic;

namespace MinAPI.Models
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
        public string ApplicationUri { get; set; }
        public string ApplicationName { get; set; }
        public int ApplicationType { get; set; }
        public string ProductUri { get; set; }
        public string ServerCapabilities { get; set; }
        public byte[] Certificate { get; set; }
        public byte[] HttpsCertificate { get; set; }
        public int? TrustListId { get; set; }
        public int? HttpsTrustListId { get; set; }

        public CertificateStores HttpsTrustList { get; set; }
        public CertificateStores TrustList { get; set; }
        public ICollection<ApplicationNames> ApplicationNames { get; set; }
        public ICollection<CertificateRequests> CertificateRequests { get; set; }
        public ICollection<ServerEndpoints> ServerEndpoints { get; set; }
    }
}
