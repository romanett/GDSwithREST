using Opc.Ua.Gds.Server;
using System.Security.Cryptography.X509Certificates;

namespace GDSwithREST.Domain.Services
{
    public interface ICertificateGroupService : ICertificateGroup
    {
        public List<CertificateGroup> CertificateGroups { get; }
        public Task<X509Certificate2Collection> GetTrustList(CertificateGroup certificateGroup);
    }
}
