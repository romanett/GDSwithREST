using Opc.Ua.Gds.Server;

namespace GDSwithREST.Services.GdsBackgroundService.Databases
{
    public class CertificateGroupDb: CertificateGroup, ICertificateGroupDb
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
    }
}
