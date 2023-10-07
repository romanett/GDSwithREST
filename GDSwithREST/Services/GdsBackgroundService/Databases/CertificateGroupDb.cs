using Opc.Ua;
using Opc.Ua.Gds.Server;
using System.Security.Cryptography.X509Certificates;

namespace GDSwithREST.Services.GdsBackgroundService.Databases
{
    public class CertificateGroupDb : CertificateGroup, ICertificateGroupDb
    {
        public List<ICertificateGroup> CertificateGroups { get; } = new List<ICertificateGroup>();

        public override CertificateGroup Create(
            string storePath,
            CertificateGroupConfiguration certificateGroupConfiguration)
        {
            var cg = new CertificateGroupDb(storePath, certificateGroupConfiguration);
            CertificateGroups.Add(cg);
            return cg;
        }

        protected CertificateGroupDb(
            string authoritiesStorePath,
            CertificateGroupConfiguration certificateGroupConfiguration
            )
            :base(authoritiesStorePath,
            certificateGroupConfiguration){}

        public async Task<X509Certificate2Collection> GetTrustList(ICertificateGroup certificateGroup)
        {
            using (ICertificateStore store = CertificateStoreIdentifier.OpenStore(certificateGroup.Configuration.TrustedListPath))
            {
                return await store.Enumerate();
            }

        }
    }
}
