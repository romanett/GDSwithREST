using Opc.Ua;
using Opc.Ua.Gds.Server;
using System.Security.Cryptography.X509Certificates;

namespace GDSwithREST.Domain.Services
{
    public class CertificateGroupService : CertificateGroup, ICertificateGroupService
    {
        public List<CertificateGroupService> CertificateGroups { get; } = new List<CertificateGroupService>();

        public override CertificateGroupService Create(
            string storePath,
            CertificateGroupConfiguration certificateGroupConfiguration)
        {
            var cg = new CertificateGroupService(storePath, certificateGroupConfiguration);
            CertificateGroups.Add(cg);
            return cg;
        }

        public CertificateGroupService() : base() { }

        protected CertificateGroupService(
            string authoritiesStorePath,
            CertificateGroupConfiguration certificateGroupConfiguration
            )
            : base(authoritiesStorePath,
            certificateGroupConfiguration)
        { }

        public async Task<X509Certificate2Collection> GetTrustList()
        {
            using (ICertificateStore store = CertificateStoreIdentifier.OpenStore(Configuration.TrustedListPath))
            {
                {
                    return await store.Enumerate();
                }
            }
        }
    }
}
