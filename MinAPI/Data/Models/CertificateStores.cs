using System;
using System.Collections.Generic;

namespace GDSwithREST.Data.Models
{
    public partial class CertificateStores
    {
        public CertificateStores()
        {
            ApplicationsHttpsTrustList = new HashSet<Applications>();
            ApplicationsTrustList = new HashSet<Applications>();
        }

        public int Id { get; set; }
        public string Path { get; set; } = null!;
        public string AuthorityId { get; set; } = null!;

        public ICollection<Applications> ApplicationsHttpsTrustList { get; set; }
        public ICollection<Applications> ApplicationsTrustList { get; set; }
    }
}
