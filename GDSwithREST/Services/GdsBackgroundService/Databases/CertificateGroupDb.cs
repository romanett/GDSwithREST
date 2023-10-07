using Opc.Ua;
using Opc.Ua.Gds.Server;
using System.Security.Cryptography.X509Certificates;

namespace GDSwithREST.Services.GdsBackgroundService.Databases
{
    public class CertificateGroupDb : CertificateGroup, ICertificateGroupDb
    {
        public List<ICertificateGroupDb> CertificateGroups { get; } = new List<ICertificateGroupDb>();

        public override CertificateGroup Create(
            string storePath,
            CertificateGroupConfiguration certificateGroupConfiguration)
        {
            var cg = new CertificateGroupDb(storePath, certificateGroupConfiguration);
            CertificateGroups.Add(cg);
            return cg;
        }

        public CertificateGroupDb() : base() { }

        protected CertificateGroupDb(
            string authoritiesStorePath,
            CertificateGroupConfiguration certificateGroupConfiguration
            )
            :base(authoritiesStorePath,
            certificateGroupConfiguration)
        { }

        public async Task<X509Certificate2Collection> GetTrustList()
        {
            using (ICertificateStore store = CertificateStoreIdentifier.OpenStore(Configuration.TrustedListPath))
            {
                return await store.Enumerate();
            }

        }
    }
}
