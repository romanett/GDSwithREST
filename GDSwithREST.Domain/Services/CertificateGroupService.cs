using Opc.Ua;
using Opc.Ua.Gds.Server;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;

namespace GDSwithREST.Domain.Services
{
    public class CertificateGroupService : CertificateGroup, ICertificateGroupService
    {
        public List<CertificateGroupService> CertificateGroups { get; } = new List<CertificateGroupService>();

        public override CertificateGroupService Create(
            string storePath,
            CertificateGroupConfiguration certificateGroupConfiguration,
            [Optional] string trustedIssuerCertificatesStorePath)
        {
            var cg = new CertificateGroupService(storePath, certificateGroupConfiguration, trustedIssuerCertificatesStorePath);
            CertificateGroups.Add(cg);
            return cg;
        }

        public CertificateGroupService() : base() { }

        protected CertificateGroupService(
            string authoritiesStorePath,
            CertificateGroupConfiguration certificateGroupConfiguration,
            [Optional] string trustedIssuerCertificatesStorePath
            )
            : base(authoritiesStorePath,
            certificateGroupConfiguration,
            trustedIssuerCertificatesStorePath)
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
