using Opc.Ua.Gds.Server;

namespace GDSwithREST.Services.GdsBackgroundService.Databases
{
    public interface ICertificateGroupDb : ICertificateGroup
    {
        public List<CertificateGroup> CertificateGroups { get; }
    }
}
