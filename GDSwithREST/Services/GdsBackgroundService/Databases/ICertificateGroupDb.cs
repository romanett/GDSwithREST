using Opc.Ua.Gds.Server;
using System.Security.Cryptography.X509Certificates;

namespace GDSwithREST.Services.GdsBackgroundService.Databases
{
    public interface ICertificateGroupDb : ICertificateGroup
    {
        public List<ICertificateGroup> CertificateGroups { get; }
        public Task<X509Certificate2Collection> GetTrustList(ICertificateGroup certificateGroup);
    }
}
