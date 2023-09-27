using Opc.Ua;
using Opc.Ua.Gds.Server;
using System.Security.Cryptography.X509Certificates;

namespace GDSwithREST.Services.GdsBackgroundService.Databases
{
    public class CertificateGroupDb : CertificateGroup, ICertificateGroupDb
    {
        public List<CertificateGroup> CertificateGroups { get; } = new List<CertificateGroup>();

        public override CertificateGroup Create(
            string storePath,
            CertificateGroupConfiguration certificateGroupConfiguration)
        {
            var cg = new CertificateGroup().Create(storePath, certificateGroupConfiguration);
            CertificateGroups.Add(cg);
            return cg;
        }

        public async Task<X509Certificate2Collection> GetTrustList(CertificateGroup certificateGroup)
        {
            using (ICertificateStore store = CertificateStoreIdentifier.OpenStore(certificateGroup.Configuration.TrustedListPath))
            {
                return await store.Enumerate();
            }

        }
    }
}
